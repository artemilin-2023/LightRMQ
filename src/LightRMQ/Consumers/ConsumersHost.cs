using LightRMQ.Configuration.Models;
using LightRMQ.Consumers.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LightRMQ.Consumers;

internal class ConsumersHost : IHostedService
{
    private readonly LightRmqConfiguration _configuration;
    private readonly Dictionary<string, IConsumer> _consumers = [];
    private readonly IServiceProvider _services;
    private readonly ILogger<ConsumersHost> _logger;

    public ConsumersHost(LightRmqConfiguration configuration, IServiceProvider services, ILogger<ConsumersHost> logger)
    {
        _configuration = configuration;
        _services = services;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var args in _configuration.ConsumerRegistrations)
            {
                var consumer = _services.GetRequiredService<Consumer>();

                var startConsumingMethod = typeof(Consumer)
                    .GetMethod(nameof(Consumer.StartConsumingAsync))
                    ?? throw new InvalidOperationException($"Cannot retrieve method '{nameof(Consumer.StartConsumingAsync)}'");
                startConsumingMethod = startConsumingMethod.MakeGenericMethod(args.MessageType);

                var consumingTag = await (Task<string>)(startConsumingMethod.Invoke(consumer, [args, cancellationToken])
                    ?? throw new InvalidOperationException($"Cannot invoke method '{nameof(Consumer.StartConsumingAsync)}'"));

                _consumers.Add(consumingTag, consumer);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to start consuming: {msg}", ex.Message);
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var (tag, consumer) in _consumers)
        {
            await consumer.StopConsumingAsync(tag, cancellationToken);
        }
    }
}
