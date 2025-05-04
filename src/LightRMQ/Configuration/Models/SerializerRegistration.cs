using LightRMQ.Abstractions;

namespace LightRMQ.Configuration.Models;

public class SerializerRegistration
{
    public required IRmqMessageSerializer Serializer { get; set; }
}
