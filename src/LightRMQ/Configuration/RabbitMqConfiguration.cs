namespace LightRMQ.Configuration;

internal record RabbitMqConfiguration
{
    public RabbitMqOptions Options { get; }
    public IReadOnlyList<ConsumerRegistration> ConsumerRegistrations { get; }
    public IReadOnlyList<SerializerRegistration> SerializerRegistrations { get; }

    internal RabbitMqConfiguration(
        RabbitMqOptions options,
        List<ConsumerRegistration> consumerRegistrations,
        List<SerializerRegistration> serializerRegistrations)
    {
        Options = options;
        ConsumerRegistrations = consumerRegistrations;
        SerializerRegistrations = serializerRegistrations;
    }
}