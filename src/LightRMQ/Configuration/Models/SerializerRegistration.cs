using LightRMQ.Abstraction;

namespace LightRMQ.Configuration.Models;

public class SerializerRegistration
{
    public required IMessageSerializer Serializer { get; set; }
}
