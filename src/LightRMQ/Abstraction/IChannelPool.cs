using RabbitMQ.Client;

namespace LightRMQ.Abstraction;

internal interface IChannelPool
{
    Task<IChannel> GetProducerChannelAsync(CancellationToken cancellationToken);
    Task<IChannel> GetConsumerChannelAsync(CancellationToken cancellationToken);
    void ReturnChannel(IChannel channel);
}
