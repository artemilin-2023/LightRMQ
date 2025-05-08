using LightRMQ.Abstractions;
using LightRMQ.Core;
using System.Text.Json;

namespace LightRMQ.Configuration.Abstractions;

public interface ISerializerConfigurationBuilder
{
    ISerializerConfigurationBuilder Use<TSerializer>(Predicate<MessageContext>? when = default) where TSerializer : class, IRabbitMqMessageSerializer;
    ISerializerConfigurationBuilder Use<TSerializer>(TSerializer serializer, Predicate<MessageContext>? when = default) where TSerializer : class, IRabbitMqMessageSerializer;
    ISerializerConfigurationBuilder UseJsonSerializer(JsonSerializerOptions? options = default, Predicate<MessageContext>? when = default);
    ISerializerConfigurationBuilder AsDefault();
}
