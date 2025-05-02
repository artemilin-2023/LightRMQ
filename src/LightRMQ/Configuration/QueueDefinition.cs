namespace LightRMQ.Configuration;

public class QueueDefinition
{
    public required string Name { get; set; }
    public bool Durable { get; set; }
    public bool Exclusive { get; set; }
    public bool AutoDelete { get; set; }
    public IDictionary<string, object?> Arguments { get; } = new Dictionary<string, object?>();
}
