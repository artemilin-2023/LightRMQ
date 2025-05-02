using LightRMQ.Abstraction;

namespace LightRMQ.Configuration;

public class SerializerRegistration
{
    public required IMessageSerializer Serializer { get; set; }
}
