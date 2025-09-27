using LightRMQ.Configuration.Models;
using LightRMQ.Consumers;
using System.Reflection;

namespace LightRMQ.Configuration.Abstractions;

public interface IConsumerConfigurationBuilder
{
    IConsumerConfigurationBuilder Register<TMessage>(Handler<TMessage> handler, Action<ConsumerRegistration>? options = default);
    IConsumerConfigurationBuilder LoadFrom(Assembly assembly);
    IConsumerConfigurationBuilder LoadFromAssemblyContains<TMarker>();

    internal IReadOnlyList<ConsumerRegistration> Build();
}
