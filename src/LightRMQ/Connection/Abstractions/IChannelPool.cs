using RabbitMQ.Client;

namespace LightRMQ.Connection.Abstractions;

internal interface IChannelPool
{
    Task<IChannel> AcquireChannelAsync(CancellationToken cancellationToken);
    Task ReturnChannelAsync(IChannel channel);
}
