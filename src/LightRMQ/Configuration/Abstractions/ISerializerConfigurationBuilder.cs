using LightRMQ.Configuration.Models;
using LightRMQ.Core;
using System.Text.Json;

namespace LightRMQ.Configuration.Abstractions;

public interface ISerializerConfigurationBuilder
{
    ISerializerConfigurationBuilder Use<TSerializer>(Predicate<ContextArgs>? when = default) where TSerializer : class, IRabbitMqMessageSerializer;
    ISerializerConfigurationBuilder Use<TSerializer>(TSerializer serializer, Predicate<ContextArgs>? when = default) where TSerializer : class, IRabbitMqMessageSerializer;
    ISerializerConfigurationBuilder UseJsonSerializer(JsonSerializerOptions? options = default, Predicate<ContextArgs>? when = default);
    ISerializerConfigurationBuilder AsDefault();

    internal SerializerConfiguration Build();
}
