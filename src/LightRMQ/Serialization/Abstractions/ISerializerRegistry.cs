using LightRMQ.Abstractions;
using LightRMQ.Core;

namespace LightRMQ.Serialization.Abstractions;

internal interface ISerializerRegistry
{
    public IRabbitMqMessageSerializer DefualtSerializer { get; }
    public IRabbitMqMessageSerializer GetSerializerByContext(MessageContext context);
    public void RegisterSerializer(IRabbitMqMessageSerializer serializer, Predicate<MessageContext> predicate);
}