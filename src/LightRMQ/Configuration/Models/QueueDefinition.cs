namespace LightRMQ.Configuration.Models;

public class QueueDefinition
{
    internal string Name { get; init; }
    internal bool Durable { get; private set; } = false;
    internal bool Exclusive { get; private set; } = false;
    internal bool AutoDelete { get; private set; } = false;
    internal IDictionary<string, object?> Arguments { get; private set; } = new Dictionary<string, object?>();

    internal QueueDefinition(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        Name = name;
    }

    public QueueDefinition AsDurable(bool durable = true)
    {
        Durable = durable;
        return this;
    }

    public QueueDefinition AsExclusive(bool exclusive = true)
    {
        Exclusive = exclusive;
        return this;
    }

    public QueueDefinition AsAutoDelete(bool autoDelete = true)
    {
        AutoDelete = autoDelete;
        return this;
    }

    public QueueDefinition WithArgs(IDictionary<string, object?> args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        Arguments = args;
        return this;
    }
}
