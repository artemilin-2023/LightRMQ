using LightRMQ.Core;

namespace LightRMQ.Consumers;

public delegate Task Handler<TMessage>(TMessage message, ReceivedMessageContext context, CancellationToken cancellationToken);