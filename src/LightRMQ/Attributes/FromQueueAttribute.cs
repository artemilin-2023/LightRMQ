namespace LightRMQ;

[AttributeUsage(AttributeTargets.Class)]
public sealed class FromQueueAttribute(string queue) : Attribute
{
    public string Queue { get; private init; } = queue;
}
