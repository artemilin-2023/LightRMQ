using LightRMQ.Core;

namespace LightRMQ.Configuration.Models;

internal class SerializerRegistration
{
    public required Type SerializerType { get; set; }
    public required Predicate<MessageContext> Predicate { get; set; }
}
