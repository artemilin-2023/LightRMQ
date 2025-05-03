using RabbitMQ.Client;

namespace LightRMQ.Abstractions;

internal interface IConnectionManager
{
    public Task<IConnection> GetOrCreateConnectionAsync(CancellationToken cancellationToken);
}
