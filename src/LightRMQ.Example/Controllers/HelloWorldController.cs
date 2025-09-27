using LightRMQ.Example.Models;
using Microsoft.AspNetCore.Mvc;

namespace LightRMQ.Example.Controllers;

[ApiController]
public class HelloWorldController(IRabbitMqClient rabbitMqClient) : Controller
{
    private readonly IRabbitMqClient _rabbitMqClient = rabbitMqClient;

    [HttpPost("/message")]
    public async Task<IActionResult> SendHello([FromBody] string text)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));

        var message = new SimpleMessage()
        {
            Text = text
        };

        await _rabbitMqClient.PublishAsync(message, CancellationToken.None,
            options => options
                .WithExchange("")
                .WithRoutingKey("example.simple-message.print-handler")
                .WithSerializer<CustomJsonSerializer>()
        );

        return Ok();
    }
}