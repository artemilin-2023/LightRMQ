using RabbitMQ.Client;

namespace LightRMQ.Abstraction;

public interface IRabbitMqTopologyManager
{
    Task EnsureTopologyAsync(CancellationToken cancellationToken);
}