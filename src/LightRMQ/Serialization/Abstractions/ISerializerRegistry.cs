using LightRMQ.Core;

namespace LightRMQ.Serialization.Abstractions;

internal interface ISerializerRegistry
{
    public IRabbitMqMessageSerializer DefualtSerializer { get; }
    public IRabbitMqMessageSerializer GetByContextOrDefault(ContextArgs context);
    public IRabbitMqMessageSerializer? Get(Type serializerType);
    public IRabbitMqMessageSerializer GetOrDefault(Type? serializerType);
    public void RegisterSerializer(IRabbitMqMessageSerializer serializer, Predicate<ContextArgs> predicate);
}