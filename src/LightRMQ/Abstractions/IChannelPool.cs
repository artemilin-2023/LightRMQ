using RabbitMQ.Client;

namespace LightRMQ.Abstractions;

internal interface IChannelPool
{
    Task<IChannel> AcquireProducerChannelAsync(CancellationToken cancellationToken);
    Task<IChannel> GetConsumerChannelAsync(CancellationToken cancellationToken);
    Task ReturnProducerChannelAsync(IChannel channel);
}
