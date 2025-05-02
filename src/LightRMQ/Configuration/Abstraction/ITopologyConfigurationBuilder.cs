using LightRMQ.Configuration.Models;

namespace LightRMQ.Configuration.Abstraction;

public interface ITopologyConfigurationBuilder
{
    ITopologyConfigurationBuilder Exchange(string exchange, string exchangeType, Action<ExchangeDefinition>? action = default);
    ITopologyConfigurationBuilder Queue(string queue, Action<QueueDefinition>? action = default);
    ITopologyConfigurationBuilder BindQueue(string queue, string exchange, string routingKey = "", Action<BindingDefinition>? action = default);
}
