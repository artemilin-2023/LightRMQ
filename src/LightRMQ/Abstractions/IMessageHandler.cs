using LightRMQ.Core;

namespace LightRMQ;

public interface IMessageHandler<TMessage>
{
    public Task HandleAsync(TMessage message, ReceivedMessageContext context, CancellationToken cancellationToken);
}
