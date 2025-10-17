using LightRMQ.Configuration.Models;

namespace LightRMQ.Configuration.Abstractions;

public interface ITopologyConfigurationBuilder
{
    ITopologyConfigurationBuilder Exchange(string exchange, string exchangeType, Action<ExchangeDefinition>? options = default);
    ITopologyConfigurationBuilder Queue(string queue, Action<QueueDefinition>? options = default);
    ITopologyConfigurationBuilder BindQueue(string queue, string toExchange, string routingKey = "", Action<BindingDefinition>? options = default);

    internal TopologyConfiguration Build();
}
