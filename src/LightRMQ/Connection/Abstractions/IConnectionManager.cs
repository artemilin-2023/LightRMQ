using RabbitMQ.Client;

namespace LightRMQ.Connection.Abstractions;

internal interface IConnectionManager
{
    public Task<IConnection> GetOrCreateConnectionAsync(CancellationToken cancellationToken);
}
