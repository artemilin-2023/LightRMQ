using LightRMQ.Abstractions;
using Microsoft.Extensions.Hosting;

namespace LightRMQ.Configuration;
internal class ConfigurationApplyerHostedService(IRabbitMqTopologyManager topologyManager) : IHostedService
{
    private readonly IRabbitMqTopologyManager _topologyManager = topologyManager;

    public async Task StartAsync(CancellationToken cancellationToken)
        => await _topologyManager.EnsureTopologyAsync(cancellationToken);


    public async Task StopAsync(CancellationToken cancellationToken)
        => await _topologyManager.DisposeAsync();
}
