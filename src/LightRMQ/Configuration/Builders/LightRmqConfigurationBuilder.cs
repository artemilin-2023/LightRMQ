using LightRMQ.Configuration.Abstractions;
using LightRMQ.Configuration.Models;

namespace LightRMQ.Configuration.Builders;

internal class LightRmqConfigurationBuilder :
    ILightRmqConfigurationBuilder
{
    private readonly ITopologyConfigurationBuilder _topologyConfigurationBuilder;
    private readonly ISerializerConfigurationBuilder _serializerConfigurationBuilder;
    private readonly IConsumerConfigurationBuilder _consumerConfiguratoinBuilder;
    private string? _connectionString;

    public LightRmqConfigurationBuilder(ITopologyConfigurationBuilder topologyConfigurationBuilder, ISerializerConfigurationBuilder serializerConfigurationBuilder, IConsumerConfigurationBuilder consumerConfiguratonBuilder)
    {
        _topologyConfigurationBuilder = topologyConfigurationBuilder;
        _serializerConfigurationBuilder = serializerConfigurationBuilder;
        _consumerConfiguratoinBuilder = consumerConfiguratonBuilder;
    }

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

    public ILightRmqConfigurationBuilder Serializers(Action<ISerializerConfigurationBuilder> serializersOptions)
    {
        ArgumentNullException.ThrowIfNull(serializersOptions);
        
        serializersOptions(_serializerConfigurationBuilder);
        return this;
    }

    public ILightRmqConfigurationBuilder Handlers(Action<IConsumerConfigurationBuilder> consumingOptions)
    {
        ArgumentNullException.ThrowIfNull(consumingOptions);

        consumingOptions(_consumerConfiguratoinBuilder);
        return this;
    }

    public LightRmqConfiguration Build()
    {
        var rmqOptions = new RabbitMqOptions()
        {
            ConnectionString = _connectionString ?? throw new InvalidOperationException("Connection string must be set before building configuration."),
            Topology = _topologyConfigurationBuilder.Build()
        };

        var serializerConfiguration = _serializerConfigurationBuilder.Build();
        var consumingConfiguration = _consumerConfiguratoinBuilder.Build();

        var configuration = new LightRmqConfiguration(rmqOptions, consumingConfiguration, serializerConfiguration);

        return configuration;
    }
}