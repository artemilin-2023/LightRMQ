namespace LightRMQ.Configuration.Models;

internal record TopologyConfiguration
{
    internal required IReadOnlyDictionary<string, ExchangeDefinition> Exchanges { get; init; }
    internal required IReadOnlyDictionary<string, QueueDefinition> Queues { get; init; }
    internal required IReadOnlyList<BindingDefinition> Bindings { get; init; }
}
