using RabbitMQ.Client;

namespace LightRMQ.Abstraction;

public interface IRabbitMqConnection
{
    Task<IChannel> GetChannelAsync(CancellationToken cancellationToken);
    Task EnsureTopologyAsync(CancellationToken cancellationToken);
}