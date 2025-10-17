using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LightRMQ.Core;

public class ContextArgs(BasicDeliverEventArgs args, Type messageType, string queue)
{
    public Type TargetMessageType { get; private init; } = messageType;
    public string? ContentType { get; private init; } = args.BasicProperties.ContentType;
    public string? ContentEncoding { get; private init; } = args.BasicProperties.ContentEncoding;
    public IDictionary<string, object?>? Headers { get; private init; } = args.BasicProperties.Headers;
    public long PayloadSize { get; private init; } = args.Body.Length;

    public string? MessageId { get; private init; } = args.BasicProperties.MessageId;
    public string? CorrelationId { get; private init; } = args.BasicProperties.CorrelationId;
    public string? ReplyTo { get; private init; } = args.BasicProperties.ReplyTo;
    public string Exchange { get; private init; } = args.Exchange;
    public string RoutingKey { get; private init; } = args.RoutingKey;
    public string Queue { get; private init; } = queue;

    public ulong DeliveryTag { get; private init; } = args.DeliveryTag;
    public bool Redelivered { get; private init; } = args.Redelivered;
    //public int RetryCount { get; private init; }

    public byte Priority { get; private init; } = args.BasicProperties.Priority;
    public AmqpTimestamp? Timestamp { get; private init; } = args.BasicProperties.Timestamp;
    public string? Type { get; private init; } = args.BasicProperties.Type;
    public string? AppId { get; private init; } = args.BasicProperties.AppId;
    public string? UserId { get; private init; } = args.BasicProperties.UserId;
    public string? ClusterId { get; private init; } = args.BasicProperties.ClusterId;

    public CancellationToken CancellationToken { get; } = args.CancellationToken;
}
