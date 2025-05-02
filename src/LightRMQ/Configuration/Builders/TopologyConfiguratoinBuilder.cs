using LightRMQ.Configuration.Abstraction;
using LightRMQ.Configuration.Models;

namespace LightRMQ.Configuration.Builders;

internal class TopologyConfiguratoinBuilder : ITopologyConfigurationBuilder
{
    public ITopologyConfigurationBuilder BindQueue(string queue, string exchange, string routingKey = "", Action<BindingDefinition>? action = null)
    {
        throw new NotImplementedException();
    }

    public ITopologyConfigurationBuilder Exchange(string exchange, string exchangeType, Action<ExchangeDefinition>? action = null)
    {
        throw new NotImplementedException();
    }

    public ITopologyConfigurationBuilder Queue(string queue, Action<QueueDefinition>? action = null)
    {
        throw new NotImplementedException();
    }

    public TopologyConfiguration Build()
    {
        throw new NotImplementedException();
    } 
}
