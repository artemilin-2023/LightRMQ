namespace LightRMQ.Configuration;

public class RabbitMqOptions
{
    public required string ConnectionString { get; set; }
    public string? ClientName { get; set; }
    public bool AutomaticRecovery { get; set; }
    public TimeSpan Heartbeat { get; set; }
    public int PrefetchCount { get; set; }
    public TimeSpan ConnectionTimeout { get; set; }
    public IDictionary<string, ExchangeDefinition> Exchanges { get; } = new Dictionary<string, ExchangeDefinition>();
    public IDictionary<string, QueueDefinition> Queues { get; } = new Dictionary<string, QueueDefinition>();
    public IList<BindingDefinition> Bindings { get; } = [];
}
