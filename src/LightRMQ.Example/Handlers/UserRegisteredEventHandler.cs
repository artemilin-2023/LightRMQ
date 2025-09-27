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

        var msg = new SimpleMessage()
        {
            Text = $"Hello, {message.Name}!"
        };

        var color = message.Age % 2 == 0 
            ? "red" 
            : "green";

        await _publisher.PublishAsync(msg, cancellationToken,
            ops => ops
                .WithExchange("amq.headers")
                .WithHeader("color", color)
        );
    }
}
