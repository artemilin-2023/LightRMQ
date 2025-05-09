using LightRMQ.Connection.Abstractions;
using LightRMQ.Core;
using LightRMQ.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace LightRMQ.Clients;

internal class RabbitMqClient(IChannelPool channelPool, ISerializerRegistry serializerRegistry, ILogger<RabbitMqClient> logger) :
    IRabbitMqClient
{
    private readonly IChannelPool _channelPool = channelPool;
    private readonly ISerializerRegistry _serializerRegistry = serializerRegistry;
    private readonly ILogger<RabbitMqClient> _logger = logger;

    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken, Action<PublishOptions>? options = default)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        var opts = new PublishOptions();
        options?.Invoke(opts);

        var channel = await _channelPool.AcquireProducerChannelAsync(cancellationToken);
        try
        {
            await PublishAsync(message, channel, opts, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message with type: {type}", message.GetType().Name);
            throw;
        }
        finally
        {
            await _channelPool.ReturnProducerChannelAsync(channel);
        }
    }

    private async Task PublishAsync<TMessage>(TMessage message, IChannel channel, PublishOptions options, CancellationToken cancellationToken)
    {
        var serializer = _serializerRegistry.GetOrDefault(options.SerializerType);
        var payload = serializer.Serialize(message);

        var properties = new BasicProperties()
        {
            ContentType = serializer.ContentType,
            ContentEncoding = serializer.ContentEncoding,
            Priority = options.Priority
        };

        await channel.BasicPublishAsync(
            exchange: options.Exchange,
            routingKey: options.RoutingKey,
            mandatory: false,
            basicProperties: properties,
            body: payload,
            cancellationToken: cancellationToken
        );

        _logger.LogDebug("Publised message with type {type}", message!.GetType().Name);
    }
}
