namespace LightRMQ.Configuration;

public class ExchangeDefinition
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public bool Durable { get; set; }
    public bool AutoDelete { get; set; }
    public IDictionary<string, object?> Arguments { get; } = new Dictionary<string, object?>();
}
