using LightRMQ.Configuration.Builders;
using LightRMQ.Configuration.Models;
using LightRMQ.Consumers;

namespace LightRMQ.Configuration.Abstractions;
public interface IConsumerConfigurationBuilder
{
    ConsumerConfigurationBuilder Register<TMessage>(Handler<TMessage> handler, Action<ConsumerRegistration>? options = default);

    internal IReadOnlyList<ConsumerRegistration> Build();
}
