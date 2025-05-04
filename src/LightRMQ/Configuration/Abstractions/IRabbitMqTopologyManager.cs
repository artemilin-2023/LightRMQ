namespace LightRMQ.Configuration.Abstractions;

public interface IRabbitMqTopologyManager : IAsyncDisposable
{
    Task EnsureTopologyAsync(CancellationToken cancellationToken);
}