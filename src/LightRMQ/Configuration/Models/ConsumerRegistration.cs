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
        private set => _queue = value ?? throw new ArgumentNullException(nameof(value)); 
    }
    
    internal Handler<object> Handler 
    { 
        get => _handler ?? throw new InvalidOperationException();
        init => _handler = value ?? throw new ArgumentNullException(nameof(value)); 
    }

    internal bool AutoAck { get; set; } = true;

    private Type? _messageType;
    private string? _queue;
    private Handler<object>? _handler;

    public ConsumerRegistration WithQueue(string queue)
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