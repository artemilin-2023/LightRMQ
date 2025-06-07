using LightRMQ.Configuration.Abstractions;
using LightRMQ.Configuration.Models;
using LightRMQ.Consumers;

namespace LightRMQ.Configuration.Builders;

public class ConsumerConfigurationBuilder :
    IConsumerConfigurationBuilder
{
    private readonly List<ConsumerRegistration> _registratons = [];

    public ConsumerConfigurationBuilder Register<TMessage>(Handler<TMessage> handler, Action<ConsumerRegistration>? options = default)
    {
        var consumerParams = new ConsumerRegistration
        {
            MessageType = typeof(TMessage),
            Handler = (message, context, cancellationToken) => handler((TMessage)message, context, cancellationToken)
        };
        options?.Invoke(consumerParams);

        _registratons.Add(consumerParams);

        return this;
    }

    public IReadOnlyList<ConsumerRegistration> Build()
        => _registratons.AsReadOnly();
}
