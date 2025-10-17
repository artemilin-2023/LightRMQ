using LightRMQ.Core;
using LightRMQ.Example.Models;

namespace LightRMQ.Example.Handlers;

[UseAutoAck(false)]
[FromQueue("example.simple-message.print-handler")]
public class SimpleMessageHandler(ILogger<SimpleMessageHandler> logger) : IMessageHandler<SimpleMessage>
{
    public async Task HandleAsync(SimpleMessage message, ReceivedMessageContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Log from handler, msg: {msg}", message.Text);

        await context.Acknowledge();
    }
}
