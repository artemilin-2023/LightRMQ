using LightRMQ.Common;
using LightRMQ.Connection.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace LightRMQ.Connection;


internal class ChannelPool :
    IChannelPool,
    IAsyncDisposable
{
    private const int PoolInitializeSize = 5;

    private static ChannelPool? _instance = null;
    internal static ChannelPool Instance => _instance ?? throw new InvalidOperationException("Instance is not set");
    
    internal static bool HasInstance => _instance is not null;

    private static readonly AsyncLocker _locker = new();

    private readonly ConnectionManager _connectionManager;
    private readonly ConcurrentBag<IChannel> _channels = [];
    private readonly CancellationTokenSource _cleanupTokenSource = new();
    private readonly Task _cleanupJob;
    private readonly ILogger<ChannelPool> _logger;
    private bool _disposed = false;

    private ChannelPool(ConnectionManager connectionManager, ILogger<ChannelPool> logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;

        _cleanupJob = Task.Run(ExecuteCleanupJobAsync);
    }

    ~ChannelPool()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }

    private async Task ExecuteCleanupJobAsync()
    {
        while (_cleanupTokenSource.IsCancellationRequested is false)
        {
            try
            {
                await CleanupUnusedChannelsAsync(_cleanupTokenSource.Token);
                await Task.Delay(TimeSpan.FromMinutes(1), _cleanupTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task CleanupUnusedChannelsAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        int counter = 0;
        using (await _locker.LockAsync(cancellationToken: cancellationToken))
        {
            foreach (var channel in _channels)
            {
                if (IsChannelValid(channel) is false)
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
    }

    internal static async Task<ChannelPool> InitializeAsync(ConnectionManager connectionManager, ILogger<ChannelPool> logger)
    {
        if (HasInstance)
            return _instance!;

        _instance = new ChannelPool(connectionManager, logger);
        for (var i = 0; i < PoolInitializeSize; i++)
        {
            var channel = await _instance.CreateChannelAsync(CancellationToken.None);
            _instance._channels.Add(channel);
        }

        return _instance;
    }

    private async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken)
    {
        using (await _locker.LockAsync(cancellationToken: cancellationToken))
        {
            var connection = await _connectionManager.GetOrCreateConnectionAsync(cancellationToken);
            var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            return channel;
        }
    }

    public async Task<IChannel> AcquireChannelAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_channels.TryTake(out var channel) && IsChannelValid(channel))
        {
            return channel;
        }

        await SafeCloseChannelAsync(channel, cancellationToken);

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
            }

            await channel.DisposeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error while closing channel: {Message}", ex.Message);
        }
    }

    public async Task ReturnChannelAsync(IChannel channel)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (channel.IsClosed)
        {
            _logger.LogWarning("Channel is not open. Disposing it.");
            await SafeCloseChannelAsync(channel, CancellationToken.None);
            return;
        }

        _channels.Add(channel);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_disposed) return;

        _cleanupTokenSource.Cancel();
        try
        {
            await _cleanupJob;
        }
        catch { }

        using (await _locker.LockAsync())
        {
            foreach (var channel in _channels)
            {
                await SafeCloseChannelAsync(channel, CancellationToken.None);
            }

            await _connectionManager.DisposeAsync();
        }

        await _locker.DisposeAsync();
        _disposed = true;
    }
}