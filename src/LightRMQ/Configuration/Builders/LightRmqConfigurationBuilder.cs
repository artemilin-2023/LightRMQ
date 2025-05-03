using LightRMQ.Configuration.Abstractions;
using LightRMQ.Configuration.Models;

namespace LightRMQ.Configuration.Builders;

internal class LightRmqConfigurationBuilder :
    ILightRmqConfigurationBuilder
{
    private readonly TopologyConfigurationBuilder _topologyConfigurationBuilder = new();
    private string? _connectionString;

    public ILightRmqConfigurationBuilder Topology(Action<ITopologyConfigurationBuilder> topology)
    {
        ArgumentNullException.ThrowIfNull(topology);

        topology(_topologyConfigurationBuilder);
        return this;
    }

    public ILightRmqConfigurationBuilder ConnectionString(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        if (Uri.IsWellFormedUriString(connectionString, UriKind.RelativeOrAbsolute) is false)
            throw new UriFormatException($"Connection string '{connectionString}' is not valid.");

        _connectionString = connectionString;
        return this;
    }

    internal LightRmqConfiguration Build()
    {
        // Пока что так, потом будет нормально без нулов и всей хуйни

        var options = new RabbitMqOptions()
        {
            ConnectionString = _connectionString ?? throw new InvalidOperationException("Connection string must be set before building configuration."),
            Topology = _topologyConfigurationBuilder.Build()
        };

        var configuration = new LightRmqConfiguration(options, null, null);

        return configuration;
    }
}