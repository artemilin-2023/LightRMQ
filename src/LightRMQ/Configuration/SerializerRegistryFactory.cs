using LightRMQ.Configuration.Models;
using LightRMQ.Serialization;
using LightRMQ.Serialization.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LightRMQ.Configuration;

internal class SerializerRegistryFactory(IServiceProvider serviceProvider, LightRmqConfiguration configuration, ILogger<SerializerRegistryFactory> logger)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly SerializerConfiguration _configuration = configuration.SerializerConfiguration;
    private readonly ILogger<SerializerRegistryFactory> _logger = logger;

    internal ISerializerRegistry Create()
    {
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        var serializerRegistry = new SerializerRegistry(loggerFactory.CreateLogger<SerializerRegistry>());

        var defaultSerializer = GetSerializer(_configuration.DefaultSerializer);
        serializerRegistry.DefualtSerializer = defaultSerializer;

        foreach (var registration in _configuration.SerializerRegistrations)
        {
            var serializer = GetSerializer(registration);
            serializerRegistry.RegisterSerializer(serializer, registration.Predicate);
        }

        _logger.LogInformation("Serializer registry created with {serializerCount} serializers. '{defaultSerializer}' is set as the default.", 
            _configuration.SerializerRegistrations.Count, _configuration.DefaultSerializer.SerializerType.Name);

        return serializerRegistry;
    }

    private IRabbitMqMessageSerializer GetSerializer(SerializerRegistration registration)
    {
        if (registration.SerializerType is null)
            throw new InvalidOperationException("Serializer type is null. Please register a serializer before.");

        return _serviceProvider.GetService(registration.SerializerType) is not IRabbitMqMessageSerializer serializer
            ? throw new InvalidOperationException($"Unable to resolve serializer of type {registration.SerializerType.Name}. Please ensure the serializer is registered in the configuration.")
            : serializer;
    }
}
