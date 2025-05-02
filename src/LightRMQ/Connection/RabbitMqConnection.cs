using LightRMQ.Abstraction;
using LightRMQ.Common;
using LightRMQ.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace LightRMQ.Connection;

internal class RabbitMqConnection(RabbitMqConfiguration configuration, ILogger<RabbitMqConnection> logger, IChannelPool channelPool) :
    IRabbitMqConnection,
    IAsyncDisposable
{
    private readonly RabbitMqConfiguration _configuration = configuration;
    private readonly ILogger<RabbitMqConnection> _logger = logger;
    private readonly IChannelPool _channelPool = channelPool;
    private readonly AsyncLocker _locker = new(1);
    private IConnection? _connection;

    public async Task EnsureTopologyAsync(CancellationToken cancellationToken)
    {
        var channel = await GetChannelAsync(cancellationToken);

        try
        {
            // Execute in this specific order without Task.WhenAll() 
            // because IChannel instances are not thread-safe.

            await ApplyExchaneDefenitionsAsync(channel, cancellationToken);
            await ApplyQueueDefenitionsAsync(channel, cancellationToken);
            await ApplyExchaneDefenitionsAsync(channel, cancellationToken);
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

            _logger.LogInformation("Declaring queue {QueueName} with durable={Durable}, exclusive={Exclusive}, autoDelete={AutoDelete}", queueName, durable, exclusive, autoDelete);

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

            _logger.LogInformation("Declaring exchange {ExchangeName} with type={Type}, durable={Durable}, autoDelete={AutoDelete}", exchangeName, type, durable, autoDelete);

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

    public async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        var connection = await GetOrCreateConnectionAsync(cancellationToken);
        var channel = _channelPool.GetChannel(connection);
        return channel;
    }

    private async Task<IConnection> GetOrCreateConnectionAsync(CancellationToken cancellationToken)
    {
        using var _ = await _locker.LockAsync();
        
        if (_connection?.IsOpen ?? false)
            return _connection;

        _logger.LogInformation("Creating new RabbitMQ connection...");

        var factory = new ConnectionFactory
        {
            Uri = new Uri(_configuration.Options.ConnectionString),
            ClientProvidedName = _configuration.Options.ClientName,
            AutomaticRecoveryEnabled = _configuration.Options.AutomaticRecovery,
            RequestedHeartbeat = _configuration.Options.Heartbeat
        };

        var connection = await factory.CreateConnectionAsync(cancellationToken);
        _connection = connection;

        _logger.LogInformation("RabbitMQ connection created.");
        return connection;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync().AsTask();
    }
}
