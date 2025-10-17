using RabbitMQ.Client;

namespace LightRMQ.Core;

public sealed record ReceivedMessageContext
{
    public ContextArgs ContextArgs { get; private init; }
    public CancellationToken CancellationToken => ContextArgs.CancellationToken;

    private readonly IChannel _channel;

    internal ReceivedMessageContext(ContextArgs contextArgs, IChannel channel)
    {
        ContextArgs = contextArgs;
        _channel = channel;
    }

    public async Task Acknowledge(bool multiply = true)
        => await _channel.BasicAckAsync(ContextArgs.DeliveryTag, multiply, CancellationToken);

    public async Task NegativeAcknowledge(bool multipy = true, bool requeu = false)
        => await _channel.BasicNackAsync(ContextArgs.DeliveryTag, multipy, requeu, CancellationToken);
}