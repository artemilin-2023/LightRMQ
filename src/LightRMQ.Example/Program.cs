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

services.AddLightRmq(config =>
{
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

    config.ConnectionString(builder.Configuration.GetConnectionString("rmq")!);

    config.Topology(topology => topology
        .Queue("example.simple-message.print-handler")
        .BindQueue(queue: "example.simple-message.print-handler", toExchange: "amq.direct")

        .Queue("example.user.registered")
        .BindQueue(queue: "example.user.registered", toExchange: "amq.direct")

        .Queue("example.simple-message.print-red")
        .BindQueue(queue: "example.simple-message.print-red", toExchange: "amq.headers", options: ops => ops.WithArgs(red))

        .Queue("example.simple-message.print-green")
        .BindQueue(queue: "example.simple-message.print-green", toExchange: "amq.headers", options: ops => ops.WithArgs(green))
    );

    config.Serializers(serializers => serializers
        .Use<CustomJsonSerializer>(when: ctx => ctx.TargetMessageType == typeof(UserRegisteredEvent))
        .UseJsonSerializer().AsDefault()
    );

    config.Handlers(handlers => handlers
        .LoadFromAssemblyContains<Program>()
        
        .Register<SimpleMessage>(
            async (msg, ctx, ct) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg.Text);
                Console.ResetColor();
            },
            ops => ops.FromQueue("example.simple-message.print-red")
        )

        .Register<SimpleMessage>(
            async (msg, ctx, ct) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(msg.Text);
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