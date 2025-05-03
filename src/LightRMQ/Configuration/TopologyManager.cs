using LightRMQ.Abstractions;
using LightRMQ.Configuration.Models;
using LightRMQ.Connection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace LightRMQ.Configuration;

internal class TopologyManager(LightRmqConfiguration configuration, ILogger<TopologyManager> logger, ChannelPool channelPool) :
    IRabbitMqTopologyManager,
    IAsyncDisposable
{
    private readonly LightRmqConfiguration _configuration = configuration;
    private readonly ILogger<TopologyManager> _logger = logger;
    private readonly ChannelPool _channelPool = channelPool;
    private bool _disposed = false;

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
        finally
        {
            await _channelPool.ReturnProducerChannelAsync(channel);
        }
    }

    private async Task ApplyQueueDefenitionsAsync(IChannel channel, CancellationToken cancellationToken)
    {
        foreach (var defenition in _configuration.Options.Topology.Queues.Values)
        {
            var queueName = defenition.Name;
            var durable = defenition.Durable;
            var exclusive = defenition.Exclusive;
            var autoDelete = defenition.AutoDelete;
            var args = defenition.Arguments;

            await channel.QueueDeclareAsync(queueName, durable, exclusive, autoDelete, arguments: args, cancellationToken: cancellationToken);
            
            _logger.LogInformation("Successfully declared queue '{QueueName}' with durable={Durable}, exclusive={Exclusive}, autoDelete={AutoDelete}.",
               queueName, durable, exclusive, autoDelete);
        }
    }

    private async Task ApplyExchaneDefenitionsAsync(IChannel channel, CancellationToken cancellationToken)
    {
        foreach (var defenition in _configuration.Options.Topology.Exchanges.Values)
        {
            var exchangeName = defenition.Name;
            var type = defenition.Type;
            var durable = defenition.Durable;
            var autoDelete = defenition.AutoDelete;
            var args = defenition.Arguments;

            await channel.ExchangeDeclareAsync(exchangeName, type, durable, autoDelete, arguments: args, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully declared exchange {ExchangeName} with type={Type}, durable={Durable}, autoDelete={AutoDelete}",
                exchangeName, type, durable, autoDelete);
        }
    }

    private async Task BindQueuesAsync(IChannel channel, CancellationToken cancellationToken)
    {
        foreach (var defenition in _configuration.Options.Topology.Bindings)
        {
            var exchangeName = defenition.ExchangeName;
            var queueName = defenition.QueueName;
            var routingKey = defenition.RoutingKey;
            var args = defenition.Arguments;

            await channel.QueueBindAsync(queueName, exchangeName, routingKey, arguments: args, cancellationToken: cancellationToken);
            
            _logger.LogInformation("Successfully bound queue '{queue}' to exchange '{exchange}' with routing key '{rk}'.",
                queueName, exchangeName, routingKey);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await _channelPool.DisposeAsync();
        _disposed = true;
    }
}
