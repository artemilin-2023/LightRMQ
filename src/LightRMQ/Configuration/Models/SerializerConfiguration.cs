namespace LightRMQ.Configuration.Models;

internal class SerializerConfiguration
{
    public required List<SerializerRegistration> SerializerRegistrations { get; set; }
    public required SerializerRegistration DefaultSerializer { get; set; }
}
