using LightRMQ.Clients;
using LightRMQ.Configuration;
using LightRMQ.Configuration.Abstractions;
using LightRMQ.Configuration.Builders;
using LightRMQ.Connection;
using LightRMQ.Connection.Abstractions;
using LightRMQ.Consumers;
using LightRMQ.Serialization;
using LightRMQ.Serialization.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightRMQ.DependencyInjection;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddLightRmq(this IServiceCollection services, Action<ILightRmqConfigurationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);
        
        services.BuildAndRegisterConfiguration(configure); 
        services.AddNetworkManagers();
        services.AddTopology();
        services.AddSerializers();
        services.AddClients();
        services.AddConsumers();

        return services;
    }

    private static IServiceCollection BuildAndRegisterConfiguration(this IServiceCollection services, Action<ILightRmqConfigurationBuilder> configure)
    {
        var topologyBuilder = new TopologyConfigurationBuilder();
        var serializerBuilder = new SerializerConfigurationBuilder(services);
        var consumerBuilder = new ConsumerConfigurationBuilder(services);

        var builder = new LightRmqConfigurationBuilder(topologyBuilder, serializerBuilder, consumerBuilder);
        configure(builder);
        var configuration = builder.Build();

        services.AddSingleton(configuration);

        return services;
    }

    private static IServiceCollection AddNetworkManagers(this IServiceCollection services)
    {
        services.AddSingleton<ChannelPool>(sp => ResolveChannelPool(sp));
        services.AddSingleton<IChannelPool, ChannelPool>(sp => ResolveChannelPool(sp));

        services.AddSingleton<ConnectionManager>();
        
        return services;
    }

    private static ChannelPool ResolveChannelPool(IServiceProvider sp)
    {
        if (ChannelPool.HasInstance)
            return ChannelPool.Instance;

        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger<ChannelPool>();
        var connectionManager = sp.GetRequiredService<ConnectionManager>();

        return ChannelPool.InitializeAsync(connectionManager, logger).GetAwaiter().GetResult();
    }

    private static IServiceCollection AddTopology(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMqTopologyManager, TopologyManager>();
        services.AddHostedService<ConfigurationApplyerHostedService>();

        return services;
    }

    private static IServiceCollection AddSerializers(this IServiceCollection services)
    {
        services.AddSingleton<SerializerRegistryFactory>();
        services.AddSingleton<ISerializerRegistry, SerializerRegistry>(sp =>
        {
            var factory = sp.GetRequiredService<SerializerRegistryFactory>();
            return (SerializerRegistry)factory.Create();
        });

        return services;
    }

    private static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddTransient<IRabbitMqClient, RabbitMqClient>();

        return services;
    }

    private static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddHostedService<ConsumersHost>();
        services.AddTransient<Consumer>();

        return services;
    }
}
