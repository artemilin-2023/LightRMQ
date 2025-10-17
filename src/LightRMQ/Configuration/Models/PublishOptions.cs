namespace LightRMQ.Core;

public class PublishOptions
{
    internal string RoutingKey { get; private set; } = "";
    internal string Exchange { get; private set; } = "amq.direct";
    internal byte Priority { get; private set; } = 0;
    internal Type? SerializerType { get; private set; }
    internal Dictionary<string, object?>? Headers { get; private set; }

    public PublishOptions WithRoutingKey(string routingKey)
    {
        ArgumentNullException.ThrowIfNull(routingKey, nameof(routingKey));

        RoutingKey = routingKey;
        return this;
    }

    public PublishOptions WithExchange(string exchange)
    {
        ArgumentNullException.ThrowIfNull(exchange, nameof(exchange));
        
        Exchange = exchange;
        return this;
    }

    public PublishOptions WithPriority(byte priority)
    {
        Priority = priority;
        return this;
    }

    public PublishOptions WithSerializer<TSerializer>()
        where TSerializer : class, IRabbitMqMessageSerializer
    {
        SerializerType = typeof(TSerializer);
        return this;
    }

    public PublishOptions WithHeader(string key, object? value)
    {
        Headers ??= [];
        Headers.Add(key, value);

        return this;
    }
}
