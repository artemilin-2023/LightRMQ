namespace LightRMQ.Configuration.Models;

internal record LightRmqConfiguration
{
    public RabbitMqOptions Options { get; }
    public IReadOnlyList<ConsumerRegistration> ConsumerRegistrations { get; }
    public SerializerConfiguration SerializerConfiguration { get; }

    internal LightRmqConfiguration(RabbitMqOptions options, IReadOnlyList<ConsumerRegistration> consumerRegistrations, SerializerConfiguration serializerConfigurations)
    {
        Options = options;
        ConsumerRegistrations = consumerRegistrations;
        SerializerConfiguration = serializerConfigurations;
    }
}