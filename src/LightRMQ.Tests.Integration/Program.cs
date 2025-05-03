using LightRMQ.DependencyInjection;
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
});

var app = builder.Build();


app.Run();