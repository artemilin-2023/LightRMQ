namespace LightRMQ.Configuration.Models;

public class BindingDefinition
{
    internal string ExchangeName { get; init; }
    internal string QueueName { get; init; }
    internal string RoutingKey { get; init; }
    internal IDictionary<string, object?> Arguments { get; private set} = new Dictionary<string, object?>();

    public BindingDefinition(string exchangeName, string queueName, string routingKey)
    {
        ArgumentNullException.ThrowIfNull(exchangeName, nameof(exchangeName));
        ArgumentNullException.ThrowIfNull(queueName, nameof(queueName));
        ArgumentNullException.ThrowIfNull(routingKey, nameof(routingKey));

        ExchangeName = exchangeName;
        QueueName = queueName;
        RoutingKey = routingKey;
    }

    public BindingDefinition WithArgs(IDictionary<string, object?> args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        Arguments = args;
        return this;
    } 
}