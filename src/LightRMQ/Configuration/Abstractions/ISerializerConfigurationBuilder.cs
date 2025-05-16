using LightRMQ.Core;
using System.Text.Json;

namespace LightRMQ.Configuration.Abstractions;

public interface ISerializerConfigurationBuilder
{
    ISerializerConfigurationBuilder Use<TSerializer>(Predicate<ReceivedMessageContext>? when = default) where TSerializer : class, IRabbitMqMessageSerializer;
    ISerializerConfigurationBuilder Use<TSerializer>(TSerializer serializer, Predicate<ReceivedMessageContext>? when = default) where TSerializer : class, IRabbitMqMessageSerializer;
    ISerializerConfigurationBuilder UseJsonSerializer(JsonSerializerOptions? options = default, Predicate<ReceivedMessageContext>? when = default);
    ISerializerConfigurationBuilder AsDefault();
}
