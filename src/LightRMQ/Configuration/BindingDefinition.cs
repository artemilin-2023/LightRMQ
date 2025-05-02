namespace LightRMQ.Configuration;

public class BindingDefinition
{
    public required string ExchangeName { get; set; }
    public required string QueueName { get; set; }
    public required string RoutingKey { get; set; }
    public IDictionary<string, object?> Arguments { get; } = new Dictionary<string, object?>();
}
