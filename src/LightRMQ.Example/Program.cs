using LightRMQ.Core;
using LightRMQ.DependencyInjection;
using LightRMQ.Example;
using LightRMQ.Example.Models;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var red = new Dictionary<string, object?>
{
    { "x-match", "all" },
    { "color", "red" },
};

var green = new Dictionary<string, object?>
{
    { "x-match", "all" },
    { "color", "green" },
};

services.AddLightRmq(config =>
{
    config.Topology(topology => topology
        .Exchange("example.exchange", ExchangeType.Fanout)

        .Queue("example.simple-message.print1")
        .BindQueue(queue: "example.simple-message.print1", toExchange: "example.exchange")

        .Queue("example.simple-message.print2")
        .BindQueue(queue: "example.simple-message.print2", toExchange: "example.exchange")

        .Queue("example.simple-message.print-red")
        .BindQueue(queue: "example.simple-message.print-red", toExchange: "amq.headers", options: ops => ops.WithArgs(red))

        .Queue("example.simple-message.print-green")
        .BindQueue(queue: "example.simple-message.print-green", toExchange: "amq.headers", options: ops => ops.WithArgs(green))
    );

    config.ConnectionString(builder.Configuration.GetConnectionString("rmq")!);

    config.Serializers(serializers => serializers
        .UseJsonSerializer(when: ctx => ctx.TargetMessageType.Namespace!.StartsWith("MyNamespace.LegacyModels"))
        .Use<CustomJsonSerializer>().AsDefault()
    );

    config.Handlers(handlers => handlers
        
        .Register(
            async (SimpleMessage msg, ReceivedMessageContext ctx, CancellationToken ct) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Received from queue: {ctx.ContextArgs.Queue}, msg: {msg.Text}");
                Console.ResetColor();
                await ctx.Acknowledge();
            }, 
            ops => ops.FromQueue("example.simple-message.print1").WithAutoAck(false)
        )

        .Register<SimpleMessage>(
            async (msg, ctx, ct) =>
            {
                await Task.Delay(10, ct);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Received from queue: {ctx.ContextArgs.Queue}, msg: {msg.Text}");
                Console.ResetColor();
            },
            ops => ops.FromQueue("example.simple-message.print2")
        )

        .Register<SimpleMessage>(
            async (msg, ctx, ct) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Received from queue: {ctx.ContextArgs.Queue}, msg: {msg.Text}");
                Console.ResetColor();
            },
            ops => ops.FromQueue("example.simple-message.print-red")
        )

        .Register<SimpleMessage>(
            async (msg, ctx, ct) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Received from queue: {ctx.ContextArgs.Queue}, msg: {msg.Text}");
                Console.ResetColor();
            },
            ops => ops.FromQueue("example.simple-message.print-green")
        )
    );
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
Console.WriteLine("Press any key...");
Console.ReadLine();