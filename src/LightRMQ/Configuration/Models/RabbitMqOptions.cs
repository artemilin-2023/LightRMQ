namespace LightRMQ.Configuration.Models;

public class RabbitMqOptions
{
    public required string ConnectionString { get; set; }
    public string? ClientName { get; set; }
    public bool AutomaticRecovery { get; set; }
    public TimeSpan Heartbeat { get; set; }
    public int PrefetchCount { get; set; }
    public TimeSpan ConnectionTimeout { get; set; }
    public TopologyConfiguration Topology { get; set; } = new TopologyConfiguration();
}