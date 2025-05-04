using LightRMQ.Abstractions;
using LightRMQ.Core;

namespace LightRMQ.Serialization.Abstractions;

internal interface ISerializerRegistry
{
    public IRmqMessageSerializer GetSerializerByContext(MessageContext context);
    public void RegisterSerializer(IRmqMessageSerializer serializer, Predicate<MessageContext> predicate);
}