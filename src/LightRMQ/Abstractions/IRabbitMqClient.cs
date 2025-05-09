using LightRMQ.Core;

namespace LightRMQ;

public interface IRabbitMqClient
{
    public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken, Action<PublishOptions>? options = default);
}
