using LightRMQ.Configuration.Models;

namespace LightRMQ.Consumers.Abstractions;

internal interface IConsumer
{
    Task<string> StartConsumingAsync<TMessage>(ConsumerRegistration consumerParams, CancellationToken cancellationToken);
    Task StopConsumingAsync(string consumingTag, CancellationToken cancellationToken);
}
