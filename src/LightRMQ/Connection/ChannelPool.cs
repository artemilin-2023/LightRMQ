using LightRMQ.Abstraction;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace LightRMQ.Connection;

internal class ChannelPool(ConnectionManager connectionManager, ILogger<ChannelPool> logger) : 
    IChannelPool,
    IAsyncDisposable
{
    private readonly ConnectionManager _connectionManager = connectionManager;
    private readonly ILogger<ChannelPool> _logger = logger;
    private readonly ConcurrentBag<IChannel> _producerChannels = [];
    private readonly ThreadLocal<IChannel> _consumerChannel = new();

    public Task<IChannel> GetConsumerChannelAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IChannel> GetProducerChannelAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void ReturnChannel(IChannel channel)
    {
        throw new NotImplementedException();
    }

    public async ValueTask DisposeAsync()
    {
        await _connectionManager.DisposeAsync();
    }
}
