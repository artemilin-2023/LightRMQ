using LightRMQ.Configuration.Abstractions;
using LightRMQ.Configuration.Models;
using LightRMQ.Consumers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LightRMQ.Configuration.Builders;

public class ConsumerConfigurationBuilder(IServiceCollection services) :
    IConsumerConfigurationBuilder
{
    private readonly List<ConsumerRegistration> _registratons = [];
    private readonly IServiceCollection _services = services;

    public IConsumerConfigurationBuilder Register<TMessage>(Handler<TMessage> handler, Action<ConsumerRegistration>? options = default)
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

    public IConsumerConfigurationBuilder LoadFromAssemblyContains<TMarker>()
        => LoadFrom(typeof(TMarker).Assembly);

    public IConsumerConfigurationBuilder LoadFrom(Assembly assembly)
    {
        var handlers = assembly.GetTypes()
            .Where(
                t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
            );

        foreach (var handler in handlers)
        {
            var messageType = handler.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>))!
                .GetGenericArguments()
                .First();
            var handlerInterface = typeof(IMessageHandler<>).MakeGenericType(messageType);
            _services.AddTransient(handlerInterface, handler);

            var queue = handler.GetCustomAttribute<FromQueueAttribute>()?
                .Queue ?? throw new InvalidOperationException($"You must specify a consumption queue using '{nameof(FromQueueAttribute)}' for handler '{handler.Name}'");

            var useAutoAck = handler.GetCustomAttribute<UseAutoAckAttribute>()?
                .UseAutoAck ?? true;

            var consumerParams = new ConsumerRegistration()
            {
                MessageType = messageType,
                HandlerType = handlerInterface,
                Queue = queue,
                AutoAck = useAutoAck
            };

            _registratons.Add(consumerParams);
        }

        return this;
    }

    public IReadOnlyList<ConsumerRegistration> Build()
        => _registratons.AsReadOnly();
}
