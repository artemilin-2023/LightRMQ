namespace LightRMQ.Configuration.Models;

public class TopologyConfiguration
{
    public IDictionary<string, ExchangeDefinition> Exchanges { get; } = new Dictionary<string, ExchangeDefinition>();
    public IDictionary<string, QueueDefinition> Queues { get; } = new Dictionary<string, QueueDefinition>();
    public IList<BindingDefinition> Bindings { get; } = [];
}
