namespace LightRMQ.Configuration.Models;

public class ConsumerRegistration
{
    public required Type MessageType { get; set; }
    public required Delegate Handler { get; set; }
    public required string Queue { get; set; }
    public required string Exchange { get; set; }
    public required string RoutingKey { get; set; }
    public int Concurrency { get; set; }
    public bool AutoAck { get; set; }
}
