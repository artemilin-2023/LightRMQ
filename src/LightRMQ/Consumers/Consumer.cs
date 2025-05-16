using LightRMQ.Connection.Abstractions;
using LightRMQ.Core;
using LightRMQ.Serialization.Abstractions;
using Microsoft.Extensions.Logging;

namespace LightRMQ.Consumers;

internal class Consumer(IChannelPool channelPool, ISerializerRegistry serializerRegistry, ILogger<Consumer> logger)
{
    private readonly IChannelPool _channelPool = channelPool;
    private readonly ISerializerRegistry _serializerRegistry = serializerRegistry;
    private readonly ILogger<Consumer> _logger = logger;

    private async Task StartConsumingAsync<TMessage>(Func<TMessage, ReceivedMessageContext, CancellationToken, Task> handler)
    {
        _logger.LogInformation("Starting consuming messages of type {MessageType}", typeof(TMessage).Name);

        var channel = await _channelPool.GetConsumerChannelAsync(CancellationToken.None);
        try
        {
            await channel.BasicConsumeAsync(
                queue: "queue_name",
                autoAck: false,
                consumer: new AsyncEventingBasicConsumer(channel)
                {
                    Received = async (model, ea) =>
                    {
                        var messageContext = new ReceivedMessageContext(
                            typeof(TMessage),
                            ea.BasicProperties.ContentType,
                            ea.BasicProperties.ContentEncoding,
                            ea.BasicProperties.Headers,
                            ea.Body.Length,
                            ea.BasicProperties.MessageId,
                            ea.BasicProperties.CorrelationId,
                            ea.BasicProperties.ReplyTo,
                            ea.Exchange,
                            ea.RoutingKey,
                            ea.QueueName,
                            ea.DeliveryTag,
                            ea.Redelivered,
                            DateTime.UtcNow,
                            0, // Retry count
                            0, // Priority
                            null, // Timestamp
                            string.Empty, // Type
                            string.Empty, // AppId
                            string.Empty, // UserId
                            string.Empty, // ClusterId
                            CancellationToken.None);
                        var serializer = _serializerRegistry.GetByContext(messageContext);
                        var message = serializer.Deserialize<TMessage>(ea.Body);
                        await handler(message, messageContext, CancellationToken.None);
                    }
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while consuming messages of type {MessageType}", typeof(TMessage).Name);
            throw;
        }
    }

}
