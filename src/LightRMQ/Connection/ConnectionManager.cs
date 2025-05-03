using LightRMQ.Abstractions;
using LightRMQ.Common;
using LightRMQ.Configuration.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace LightRMQ.Connection;

internal class ConnectionManager(LightRmqConfiguration configuration, ILogger<ConnectionManager> logger) :
    IConnectionManager,
    IAsyncDisposable
{
    private readonly LightRmqConfiguration _configuration = configuration;
    private readonly ILogger<ConnectionManager> _logger = logger;
    private readonly AsyncLocker _locker = new();
    private IConnection? _connection;
    private bool _disposed = false;

    public async Task<IConnection> GetOrCreateConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await GetOrCreateConnectionProccessAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating RabbitMQ connection");
            throw;
        }
    }

    private async Task<IConnection> GetOrCreateConnectionProccessAsync(CancellationToken cancellationToken)
    {
        using (await _locker.LockAsync(cancellationToken: cancellationToken))
        {
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
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_connection is not null)
            await _connection.DisposeAsync().AsTask();

        _disposed = true;
    }
}
