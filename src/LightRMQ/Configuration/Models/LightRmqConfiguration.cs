namespace LightRMQ.Configuration.Models;

internal record LightRmqConfiguration
{
    public RabbitMqOptions Options { get; }
    public IReadOnlyList<ConsumerRegistration> ConsumerRegistrations { get; }
    public IReadOnlyList<SerializerRegistration> SerializerRegistrations { get; }

    internal LightRmqConfiguration(
        RabbitMqOptions options,
        List<ConsumerRegistration> consumerRegistrations,
        List<SerializerRegistration> serializerRegistrations)
    {
        Options = options;
        ConsumerRegistrations = consumerRegistrations;
        SerializerRegistrations = serializerRegistrations;
    }
}