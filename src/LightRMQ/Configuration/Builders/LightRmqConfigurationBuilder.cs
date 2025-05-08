using LightRMQ.Configuration.Abstractions;
using LightRMQ.Configuration.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LightRMQ.Configuration.Builders;

internal class LightRmqConfigurationBuilder(IServiceCollection services) :
    ILightRmqConfigurationBuilder
{
    private readonly TopologyConfigurationBuilder _topologyConfigurationBuilder = new();
    private readonly SerializerConfigurationBuilder _serializerConfigurationBuilder = new(services);
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

    public ILightRmqConfigurationBuilder Serializers(Action<ISerializerConfigurationBuilder> serializersBuilder)
    {
        ArgumentNullException.ThrowIfNull(serializersBuilder);
        
        serializersBuilder(_serializerConfigurationBuilder);
        return this;
    }

    internal LightRmqConfiguration Build()
    {
        // Пока что так, потом будет нормально без нулов и всей хуйни

        var rmqOptions = new RabbitMqOptions()
        {
            ConnectionString = _connectionString ?? throw new InvalidOperationException("Connection string must be set before building configuration."),
            Topology = _topologyConfigurationBuilder.Build()
        };

        var serializerConfiguration = _serializerConfigurationBuilder.Build();

        var configuration = new LightRmqConfiguration(rmqOptions, null, serializerConfiguration);

        return configuration;
    }
}