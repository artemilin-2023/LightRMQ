namespace LightRMQ.Configuration.Models;

internal class RabbitMqOptions
{
    public required string ConnectionString { get; set; }
    public string? ClientName { get; set; }
    public bool AutomaticRecovery { get; set; }
    public TimeSpan Heartbeat { get; set; }
    public int PrefetchCount { get; set; }
    public TimeSpan ConnectionTimeout { get; set; }
    internal required TopologyConfiguration Topology { get; init; }
}