using RabbitMQ.Client;

namespace LightRMQ.Abstraction;
internal interface IConnectionManager
{
    public Task<IConnection> GetOrCreateConnectionAsync(CancellationToken cancellationToken);
}
