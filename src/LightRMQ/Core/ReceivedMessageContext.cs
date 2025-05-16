namespace LightRMQ.Core;

public sealed record ReceivedMessageContext
{
    public Type TargetMessageType { get; private init; }
    public string ContentType { get; private init; }
    public string? ContentEncoding { get; private init; }
    public IDictionary<string, object?> Headers { get; private init; }
    public long PayloadSize { get; private init; }

    public string MessageId { get; private init; }
    public string CorrelationId { get; private init; }
    public string ReplyTo { get; private init; }
    public string Exchange { get; private init; }
    public string RoutingKey { get; private init; }
    public string Queue { get; private init; }

    public ulong DeliveryTag { get; private init; }
    public bool Redelivered { get; private init; }
    public DateTime ReceivedAt { get; private init; }
    public int RetryCount { get; private init; }

    public byte Priority { get; private init; }
    public DateTime? Timestamp { get; private init; }
    public string Type { get; private init; }
    public string AppId { get; private init; }
    public string UserId { get; private init; }
    public string ClusterId { get; private init; }

    public CancellationToken CancellationToken { get; }

    // God sorry...
    public ReceivedMessageContext(
        Type messageType, 
        string contentType, 
        string? contentEncoding, 
        IDictionary<string, object?> headers,
        long payloadSize,
        string messageId, 
        string correlationId, 
        string replyTo, 
        string exchange, 
        string routingKey, 
        string queue, 
        ulong deliveryTag, 
        bool redelivered, 
        DateTime receivedAt, 
        int retryCount, 
        byte priority, 
        DateTime? timestamp, 
        string type, 
        string appId, 
        string userId, 
        string clusterId, 
        CancellationToken cancellationToken)
    {
        TargetMessageType = messageType;
        ContentType = contentType;
        ContentEncoding = contentEncoding;
        Headers = headers;
        PayloadSize = payloadSize;
        MessageId = messageId;
        CorrelationId = correlationId;
        ReplyTo = replyTo;
        Exchange = exchange;
        RoutingKey = routingKey;
        Queue = queue;
        DeliveryTag = deliveryTag;
        Redelivered = redelivered;
        ReceivedAt = receivedAt;
        RetryCount = retryCount;
        Priority = priority;
        Timestamp = timestamp;
        Type = type;
        AppId = appId;
        UserId = userId;
        ClusterId = clusterId;
        CancellationToken = cancellationToken;
    }
}
