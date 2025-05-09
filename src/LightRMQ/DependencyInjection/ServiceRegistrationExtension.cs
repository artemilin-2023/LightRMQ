using LightRMQ.Clients;
using LightRMQ.Configuration;
using LightRMQ.Configuration.Abstractions;
using LightRMQ.Configuration.Builders;
using LightRMQ.Connection;
using LightRMQ.Connection.Abstractions;
using LightRMQ.Serialization;
using LightRMQ.Serialization.Abstractions;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }

    private static IServiceCollection BuildAndRegisterConfiguration(this IServiceCollection services, Action<ILightRmqConfigurationBuilder> configure)
    {
        var builder = new LightRmqConfigurationBuilder(services);
        configure(builder);
        var configuration = builder.Build();

        services.AddSingleton(configuration);

        return services;
    }

    private static IServiceCollection AddNetworkManagers(this IServiceCollection services)
    {
        services.AddSingleton<ChannelPool>();
        services.AddSingleton<IChannelPool, ChannelPool>();
        services.AddSingleton<ConnectionManager>();
        
        return services;
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
}
