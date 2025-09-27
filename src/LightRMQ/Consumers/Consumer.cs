using LightRMQ.Configuration.Models;
using LightRMQ.Connection.Abstractions;
using LightRMQ.Consumers.Abstractions;
using LightRMQ.Core;
using LightRMQ.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.Serialization;

namespace LightRMQ.Consumers;

internal class Consumer(IChannelPool channelPool, ISerializerRegistry serializerRegistry, ILogger<Consumer> logger) :
    IConsumer
{
    private readonly IChannelPool _channelPool = channelPool;
    private IChannel? _currentChannel;
    private readonly ISerializerRegistry _serializerRegistry = serializerRegistry;
    private readonly ILogger<Consumer> _logger = logger;

    public async Task<string> StartConsumingAsync<TMessage>(ConsumerRegistration consumerParams, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting consuming messages of type {MessageType} from queue '{Queue}'", 
            typeof(TMessage).Name, consumerParams.Queue);

        _currentChannel = await _channelPool.AcquireChannelAsync(cancellationToken);
        try
        {
            var consumer = new AsyncEventingBasicConsumer(_currentChannel);
            consumer.ReceivedAsync += (consumingChannel, eventArgs) =>
                OnMessageReceivedAsync<TMessage>(((AsyncEventingBasicConsumer)consumingChannel).Channel, eventArgs, consumerParams);

            var consumingTag = await _currentChannel.BasicConsumeAsync(consumerParams.Queue, consumerParams.AutoAck, consumer, cancellationToken);
            _logger.LogInformation("Consumer started. Consumer tag is '{tag}'", consumingTag);

            return consumingTag;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while consuming messages of type {MessageType}", typeof(TMessage).Name);
            throw;
        }
    }

    public async Task StopConsumingAsync(string consumingTag, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping message consumption with tag '{ConsumingTag}'", consumingTag);
        if (_currentChannel is null)
            throw new InvalidOperationException("Channel was null");

        await _currentChannel.BasicCancelAsync(consumingTag, cancellationToken: cancellationToken);
        await _channelPool.ReturnChannelAsync(_currentChannel);
    }

    private async Task OnMessageReceivedAsync<TMessage>(IChannel channel, BasicDeliverEventArgs args, ConsumerRegistration consumerParams)
    {
        _logger.LogDebug("Received message of type {MessageType} from queue '{Queue}'",
            typeof(TMessage).Name, consumerParams.Queue);

        var contextArgs = new ContextArgs(args, typeof(TMessage), consumerParams.Queue);
        var context = new ReceivedMessageContext(contextArgs, channel);
        var serializer = _serializerRegistry.GetByContextOrDefault(context.ContextArgs);

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
