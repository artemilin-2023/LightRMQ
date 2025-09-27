namespace LightRMQ;

[AttributeUsage(AttributeTargets.Class)]
public sealed class UseAutoAckAttribute(bool condition = true) : Attribute
{
    public bool UseAutoAck { get; private init; } = condition;
}
