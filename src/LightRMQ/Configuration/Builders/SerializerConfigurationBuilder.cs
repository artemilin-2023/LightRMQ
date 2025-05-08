using LightRMQ.Abstractions;
using LightRMQ.Configuration.Abstractions;
using LightRMQ.Configuration.Models;
using LightRMQ.Core;
using LightRMQ.Serialization.DefaultSerializers;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;


namespace LightRMQ.Configuration.Builders;

internal class SerializerConfigurationBuilder(IServiceCollection services) :
    ISerializerConfigurationBuilder
{
    private readonly IServiceCollection _services = services;
    private readonly List<SerializerRegistration> _serializerRegistrations = [];
    private SerializerRegistration? _defualtSerializer;

    public ISerializerConfigurationBuilder AsDefault()
    {
        _defualtSerializer = _serializerRegistrations.Last()
            ?? throw new InvalidOperationException("No serializer found. Please register a serializer before.");

        return this;
    }

    public ISerializerConfigurationBuilder Use<TSerializer>(Predicate<MessageContext>? when = null) 
        where TSerializer : class, IRabbitMqMessageSerializer
    {
        _services.AddSingleton(typeof(TSerializer));
        var registration = new SerializerRegistration
        {
            SerializerType = typeof(TSerializer),
            Predicate = when ?? (static _ => true)
        };

        _serializerRegistrations.Add(registration);
        return this;
    }

    public ISerializerConfigurationBuilder Use<TSerializer>(TSerializer serializer, Predicate<MessageContext>? when = null) 
        where TSerializer : class, IRabbitMqMessageSerializer
    {
        _services.AddSingleton(serializer);
        var registration = new SerializerRegistration
        {
            SerializerType = serializer.GetType(),
            Predicate = when ?? (static _ => true)
        };

        _serializerRegistrations.Add(registration);
        return this;
    }

    public ISerializerConfigurationBuilder UseJsonSerializer(JsonSerializerOptions? options = null, Predicate<MessageContext>? when = null)
    {
        var serializer = new RabbitMqJsonSerializer(options);
        return Use(serializer, when);
    }

    internal SerializerConfiguration Build()
    {
        if (_serializerRegistrations.Count == 0)
            throw new InvalidOperationException("No serializer registered. Please register a serializer before building the configuration.");

        if (_defualtSerializer is null && _serializerRegistrations.Count != 1)
            throw new InvalidOperationException("Default serializer not set. Please set a default serializer before building the configuration.");

        _defualtSerializer ??= _serializerRegistrations.Last();

        return new SerializerConfiguration
        {
            SerializerRegistrations = _serializerRegistrations,
            DefaultSerializer = _defualtSerializer
        };
    }
}
