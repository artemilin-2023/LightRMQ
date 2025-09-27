using LightRMQ.Configuration.Abstractions;
using LightRMQ.Configuration.Models;

namespace LightRMQ.Configuration.Builders;

internal sealed class TopologyConfigurationBuilder : 
    ITopologyConfigurationBuilder
{
    private Dictionary<string, ExchangeDefinition> Exchanges { get; } = [];
    private Dictionary<string, QueueDefinition> Queues { get; } = [];
    private List<BindingDefinition> Bindings { get; } = [];

    public ITopologyConfigurationBuilder BindQueue(string queue, string exchange, string routingKey = "", Action<BindingDefinition>? action = null)
    {
        var bindingDefinition = new BindingDefinition(exchange, queue, routingKey);
        action?.Invoke(bindingDefinition);
        Bindings.Add(bindingDefinition);

        return this;
    }

    public ITopologyConfigurationBuilder Exchange(string exchange, string exchangeType, Action<ExchangeDefinition>? action = null)
    {
        if (Exchanges.ContainsKey(exchange))
            throw new ArgumentException($"Exchange '{exchange}' already exists.");

        var exchangeDefinition = new ExchangeDefinition(exchange, exchangeType);
        action?.Invoke(exchangeDefinition);
        Exchanges.Add(exchange, exchangeDefinition);

        return this;
    }

    public ITopologyConfigurationBuilder Queue(string queue, Action<QueueDefinition>? action = null)
    {
        if (Queues.ContainsKey(queue))
            throw new ArgumentException($"Queue '{queue}' already exists.");
        
        var queueDefinition = new QueueDefinition(queue);
        action?.Invoke(queueDefinition);
        Queues.Add(queue, queueDefinition);
        
        return this;
    }

    public TopologyConfiguration Build()
    {
        var configuration = new TopologyConfiguration
        {
            Exchanges = Exchanges.AsReadOnly(),
            Queues = Queues.AsReadOnly(),
            Bindings = Bindings.AsReadOnly()
        };

        return configuration;
    }
}
