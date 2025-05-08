using LightRMQ.DependencyInjection;
using LightRMQ.Serialization.DefaultSerializers;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLightRmq(config =>
{
    config.Topology(topology => topology
        .Queue("queue", q => q.AsDurable().AsAutoDelete())
        .Exchange("hui", ExchangeType.Fanout)
        .BindQueue("queue", "hui")
    );

    config.Topology(topology => topology
        .Queue("aboba")
        .Exchange("bebra", ExchangeType.Topic)
        .BindQueue("aboba", "bebra", "asdf")
    );

    config.ConnectionString(builder.Configuration.GetConnectionString("rmq")!);

    config.Serializers(serializers => serializers
        .UseJsonSerializer(when: ctx => ctx.ContentType == "application/json").AsDefault()
        .Use<RabbitMqJsonSerializer>()
    );
});

var app = builder.Build();

app.Run();