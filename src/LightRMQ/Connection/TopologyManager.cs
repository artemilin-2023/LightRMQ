using LightRMQ.Abstraction;
using LightRMQ.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace LightRMQ.Connection;

internal class TopologyManager(RabbitMqConfiguration configuration, ILogger<TopologyManager> logger, ChannelPool channelPool) :
    IRabbitMqTopologyManager,
    IAsyncDisposable
{
    private readonly RabbitMqConfiguration _configuration = configuration;
    private readonly ILogger<TopologyManager> _logger = logger;
    private readonly ChannelPool _channelPool = channelPool;

    public async Task EnsureTopologyAsync(CancellationToken cancellationToken)
    {
        var channel = await _channelPool.AcquireProducerChannelAsync(cancellationToken);

        try
        {
            // Execute in this specific order without Task.WhenAll()
            // because IChannel instances are not thread-safe.

            await ApplyExchaneDefenitionsAsync(channel, cancellationToken);
            await ApplyQueueDefenitionsAsync(channel, cancellationToken);
            await BindQueuesAsync(channel, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring topology");
        }
    }

    private async Task ApplyQueueDefenitionsAsync(IChannel channel, CancellationToken cancellationToken)
    {
        foreach (var defenition in _configuration.Options.Queues.Values)
        {
            var queueName = defenition.Name;
            var durable = defenition.Durable;
            var exclusive = defenition.Exclusive;
            var autoDelete = defenition.AutoDelete;
            var args = defenition.Arguments;

            _logger.LogInformation("Declaring queue {QueueName} with durable={Durable}, exclusive={Exclusive}, autoDelete={AutoDelete}",
                queueName, durable, exclusive, autoDelete);

            await channel.QueueDeclareAsync(queueName, durable, exclusive, autoDelete, arguments: args, cancellationToken: cancellationToken);
        }
    }

    private async Task ApplyExchaneDefenitionsAsync(IChannel channel, CancellationToken cancellationToken)
    {
        foreach (var defenition in _configuration.Options.Exchanges.Values)
        {
            var exchangeName = defenition.Name;
            var type = defenition.Type;
            var durable = defenition.Durable;
            var autoDelete = defenition.AutoDelete;
            var args = defenition.Arguments;

            _logger.LogInformation("Declaring exchange {ExchangeName} with type={Type}, durable={Durable}, autoDelete={AutoDelete}",
                exchangeName, type, durable, autoDelete);

            await channel.ExchangeDeclareAsync(exchangeName, type, durable, autoDelete, arguments: args, cancellationToken: cancellationToken);
        }
    }

    private async Task BindQueuesAsync(IChannel channel, CancellationToken cancellationToken)
    {
        foreach (var defenition in _configuration.Options.Bindings)
        {
            var exchangeName = defenition.QueueName;
            var queueName = defenition.QueueName;
            var routingKey = defenition.RoutingKey;
            var args = defenition.Arguments;

            _logger.LogInformation("Binding queue '{queue}' to exchange '{exchange}' with routing key '{rk}'",
                queueName, exchangeName, routingKey);

            await channel.QueueBindAsync(queueName, exchangeName, routingKey, arguments: args, cancellationToken: cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _channelPool.DisposeAsync();
    }
}
