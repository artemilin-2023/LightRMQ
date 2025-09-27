using LightRMQ.Consumers;

namespace LightRMQ.Configuration.Models;

public class ConsumerRegistration
{
    internal Type MessageType 
    { 
        get => _messageType ?? throw new InvalidOperationException(); 
        init => _messageType = value ?? throw new ArgumentNullException(nameof(value)); 
    }

    internal string Queue 
    { 
        get => _queue ?? throw new InvalidOperationException(); 
        set => _queue = value ?? throw new ArgumentNullException(nameof(value)); 
    }
    
    internal Handler<object> Handler 
    { 
        get => _handler ?? throw new InvalidOperationException();
        init => _handler = value ?? throw new ArgumentNullException(nameof(value)); 
    }

    internal bool AutoAck { get; set; } = true;

    internal Type? HandlerType { get; set; }

    internal bool HandlerIsLambda => HandlerType is null;

    private Type? _messageType;
    private string? _queue;
    private Handler<object>? _handler;

    public ConsumerRegistration FromQueue(string queue)
    {
        Queue = queue;
        return this;
    }

    public ConsumerRegistration WithAutoAck(bool autoAck = true)
    {
        AutoAck = autoAck;
        return this;
    }
}