using LightRMQ.Configuration.Models;

namespace LightRMQ.Configuration.Abstractions;

public interface ILightRmqConfigurationBuilder
{
    public ILightRmqConfigurationBuilder Topology(Action<ITopologyConfigurationBuilder> topology);
    public ILightRmqConfigurationBuilder ConnectionString(string connectionString);
    public ILightRmqConfigurationBuilder Serializers(Action<ISerializerConfigurationBuilder> serializersBuilder);
    public ILightRmqConfigurationBuilder Handlers(Action<IConsumerConfigurationBuilder> consumingOptions);

    internal LightRmqConfiguration Build();
}
