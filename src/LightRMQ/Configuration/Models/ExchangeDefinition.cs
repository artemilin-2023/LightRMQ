using RabbitMQ.Client;

namespace LightRMQ.Configuration.Models;

public class ExchangeDefinition
{
    internal string Name { get; init; }
    internal string Type { get; init; }
    internal bool Durable { get; private set; } = false;
    internal bool AutoDelete { get; private set; } = false;
    internal IDictionary<string, object?> Arguments { get; private set; } = new Dictionary<string, object?>();

    internal ExchangeDefinition(string name, string type)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        if (ExchangeType.All().Contains(type) is false)
            throw new ArgumentException($"Invalid exchange type: {type}. Exchange type must be one of '{string.Join(", ", ExchangeType.All())}'", nameof(type));

        Name = name;
        Type = type;
    }

    public ExchangeDefinition AsDurable(bool durable = true)
    {
        Durable = durable;
        return this;
    }

    public ExchangeDefinition AsAutoDelete(bool autoDelete = true)
    {
        AutoDelete = autoDelete;
        return this;
    }

    public ExchangeDefinition WithArgs(IDictionary<string, object?> args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        Arguments = args;
        return this;
    }
}
