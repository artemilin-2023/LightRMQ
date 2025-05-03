namespace LightRMQ.Abstractions;

public interface IRabbitMqTopologyManager : IAsyncDisposable
{
    Task EnsureTopologyAsync(CancellationToken cancellationToken);
}