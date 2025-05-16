using LightRMQ.DependencyInjection;
using LightRMQ.Example;
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
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();