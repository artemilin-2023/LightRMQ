using LightRMQ.Core;
using LightRMQ.Example.Models;

namespace LightRMQ.Example.Handlers;

[FromQueue("example.user.registered")]
public class UserRegisteredEventHandler : IMessageHandler<UserRegisteredEvent>
{
    private readonly IRabbitMqClient _publisher;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(IRabbitMqClient publisher, ILogger<UserRegisteredEventHandler> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task HandleAsync(UserRegisteredEvent message, ReceivedMessageContext context, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User with name {name} and age {age} registered", message.Name, message.Age);

        await _publisher.PublishAsync(
            new SimpleMessage()
            {
                Text = $"Hello, {message.Name}!"
            },
            cancellationToken,
            ops => ops.WithExchange("example.exchange")
        );
    }
}
