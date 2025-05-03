using LightRMQ.Abstractions;
using LightRMQ.Common;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace LightRMQ.Connection;

internal class ChannelPool :
    IChannelPool,
    IAsyncDisposable
{
    private const int PoolInitializeSize = 5;

    private readonly ConnectionManager _connectionManager;
    private readonly ILogger<ChannelPool> _logger;
    private readonly ConcurrentBag<IChannel> _producerChannels = [];
    private readonly ThreadLocal<IChannel> _consumerChannel = new(trackAllValues: true);
    private readonly AsyncLocker _locker = new();
    private bool _disposed = false;

    public ChannelPool(ConnectionManager connectionManager, ILogger<ChannelPool> logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;

        for (var i = 0; i < PoolInitializeSize; i++)
        {
            _producerChannels.Add(CreateChannelAsync(CancellationToken.None).GetAwaiter().GetResult());
        }
    }

    public async Task<IChannel> GetConsumerChannelAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _consumerChannel.Value ??= await CreateChannelAsync(cancellationToken);
        return _consumerChannel.Value;
    }

    private async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken)
    {
        using (await _locker.LockAsync(cancellationToken: cancellationToken))
        {
            var connection = await _connectionManager.GetOrCreateConnectionAsync(cancellationToken);
            var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            _logger.LogInformation("New RabbitMQ channel opened successfully.");

            return channel;
        }
    }

    public async Task<IChannel> AcquireProducerChannelAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_producerChannels.TryTake(out var channel) && IsChannelValid(channel))
        {
            _logger.LogDebug("Reusing existing producer channel.");
            return channel;
        }

        await SafeCloseChannelAsync(channel, cancellationToken);

        _logger.LogDebug("Creating new producer channel.");
        var newChannel = await CreateChannelAsync(cancellationToken);
        return newChannel;
    }

    private static bool IsChannelValid(IChannel channel)
        => channel is not null && channel.IsOpen;

    private async Task SafeCloseChannelAsync(IChannel? channel, CancellationToken cancellationToken)
    {
        if (channel is null) return;

        try
        {
            if (channel.IsOpen)
            {
                await channel.CloseAsync(cancellationToken: cancellationToken);
                _logger.LogDebug("Channel closed successfully.");
            }

            await channel.DisposeAsync();
            _logger.LogInformation("Channel disposed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error while closing channel: {Message}", ex.Message);
        }
    }

    public async Task ReturnProducerChannelAsync(IChannel channel)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (channel.IsClosed)
        {
            _logger.LogWarning("Channel is not open. Disposing it.");
            await SafeCloseChannelAsync(channel, CancellationToken.None);
            return;
        }

        _producerChannels.Add(channel);
        _logger.LogDebug("Returned channel to pool.");
    }

    public async Task CleanupUnusedChannelsAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        int counter = 0;
        using (await _locker.LockAsync(cancellationToken: cancellationToken))
        {
            foreach (var channel in _producerChannels)
            {
                if (!IsChannelValid(channel))
                {
                    await SafeCloseChannelAsync(channel, cancellationToken);
                    counter++;
                }
            }
        }

        if (counter > 0)
        {
            _logger.LogInformation("Cleaned up {Count} unused channels.", counter);
            return;
        }

        _logger.LogDebug("All channels are in use. Nothing to clean up.");
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        using (await _locker.LockAsync())
        {

            await _connectionManager.DisposeAsync();
            foreach (var channel in _producerChannels)
            {
                await SafeCloseChannelAsync(channel, CancellationToken.None);
            }

            _consumerChannel.Dispose();
        }

        await _locker.DisposeAsync();
        _disposed = true;
    }
}