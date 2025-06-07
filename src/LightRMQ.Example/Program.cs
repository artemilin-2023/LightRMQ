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
    config.Topology(topology => topology
        .Queue("aboba")
        .Exchange("bebra", ExchangeType.Topic)
        .BindQueue(queue: "aboba", exchange: "bebra", routingKey: "asdf")
    );

    config.ConnectionString(builder.Configuration.GetConnectionString("rmq")!);

    config.Serializers(serializers => serializers
        .UseJsonSerializer(when: ctx => ctx.TargetMessageType.Namespace!.StartsWith("MyNamespace.LegacyModels"))
        .Use<CustomJsonSerializer>().AsDefault()
    );

    config.Handlers(handlers =>
        handlers.Register((SimpleMessage msg, ReceivedMessageContext ctx, CancellationToken ct) =>
        {
            Console.WriteLine(msg.Text);
            return Task.CompletedTask;
        }, ops => ops.WithQueue("aboba"))
    );
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();