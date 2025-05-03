namespace LightRMQ.Configuration.Abstractions;

public interface ILightRmqConfigurationBuilder
{
    public ILightRmqConfigurationBuilder Topology(Action<ITopologyConfigurationBuilder> topology);
    public ILightRmqConfigurationBuilder ConnectionString(string connectionString);
}
