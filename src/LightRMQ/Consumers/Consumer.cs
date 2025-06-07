using LightRMQ.Configuration.Models;
using LightRMQ.Connection.Abstractions;
using LightRMQ.Core;
using LightRMQ.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.Serialization;

namespace LightRMQ.Consumers;

internal class Consumer(IChannelPool channelPool, ISerializerRegistry serializerRegistry, ILogger<Consumer> logger)
{
    private readonly IChannelPool _channelPool = channelPool;
    private readonly ISerializerRegistry _serializerRegistry = serializerRegistry;
    private readonly ILogger<Consumer> _logger = logger;

    public async Task StartConsumingAsync<TMessage>(ConsumerRegistration consumerParams, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting consuming messages of type {MessageType} form queue '{Queue}'", 
            typeof(TMessage).Name, consumerParams.Queue);

        var channel = await _channelPool.GetConsumerChannelAsync(cancellationToken);
        try
        {
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (consumingChannel, eventArgs) =>
                OnMessageReceivedAsync<TMessage>((IChannel)consumingChannel, eventArgs, consumerParams);

            await channel.BasicConsumeAsync(consumerParams.Queue, consumerParams.AutoAck, consumer, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while consuming messages of type {MessageType}", typeof(TMessage).Name);
            throw;
        }
    }

    public async Task StopConsumingAsync(string consumingTag, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping consuming messages with tag '{ConsumingTag}'", consumingTag);

        var channel = await _channelPool.GetConsumerChannelAsync(cancellationToken);
        await channel.BasicCancelAsync(consumingTag, cancellationToken: cancellationToken);
    }

    private async Task OnMessageReceivedAsync<TMessage>(IChannel channel, BasicDeliverEventArgs args, ConsumerRegistration consumerParams)
    {
        _logger.LogDebug("Received message of type {MessageType} from queue '{Queue}'",
            typeof(TMessage).Name, consumerParams.Queue);

        var contextArgs = new ContextArgs(args, typeof(TMessage), consumerParams.Queue);
        var context = new ReceivedMessageContext(contextArgs, channel);
        var serializer = _serializerRegistry.GetByContextOrDefault(context);

        var payload = args.Body.ToArray();
        var handler = consumerParams.Handler; 
        try
        {
            var message = serializer.Deserialize<TMessage>(payload) 
                ?? throw new SerializationException($"Failed to deserialize message to type '{typeof(TMessage).Name}'");
            
            await handler.Invoke(message, context, args.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing message of type {MessageType}", typeof(TMessage).Name);
            throw;
        }
    }
}
