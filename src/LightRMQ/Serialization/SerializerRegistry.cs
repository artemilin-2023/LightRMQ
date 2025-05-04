using LightRMQ.Abstractions;
using LightRMQ.Core;
using LightRMQ.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace LightRMQ.Serialization;

internal class SerializerRegistry(IRmqMessageSerializer? defualtSerializer = null, ILogger<SerializerRegistry> logger) : ISerializerRegistry
{
    private readonly List<(Predicate<MessageContext>, IRmqMessageSerializer)> _serializers = [];
    private readonly ConcurrentDictionary<Type, IRmqMessageSerializer> _messageTypeSerializersCache = [];
    private readonly IRmqMessageSerializer? _defualtSerializer = defualtSerializer;
    private readonly ILogger<SerializerRegistry> _logger = logger;

    public IRmqMessageSerializer GetSerializerByContext(MessageContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        if (context.MessageType is not null && _messageTypeSerializersCache.TryGetValue(context.MessageType, out var cachedSerializer))
        {
            _logger.LogDebug("Using cached serializer {Serializer} for message type: {MessageType}", cachedSerializer.GetType().Name, context.MessageType.Name);
            return cachedSerializer;
        }

        _logger.LogDebug("No cached serializer found for message type: {MessageType}.", context.MessageType?.Name);
        foreach (var (predicate, serializer) in _serializers)
        {
            if (predicate(context))
            {
                if (context.MessageType is not null)
                    _messageTypeSerializersCache.TryAdd(context.MessageType, serializer);

                _logger.LogDebug("Using serializer {Serializer} for message type: {MessageType}", serializer.GetType().Name, context.MessageType!.Name);
                return serializer;
            }
        }

        _logger.LogDebug(_defualtSerializer is null ? "No matching serializer found and no default serializer set." : "No matching serializer found, using default serializer.");
        return _defualtSerializer ?? throw new Exception("No serializer found for the given context. Ensure that serializers are configured or set a default one.");
    }

    public void RegisterSerializer(IRmqMessageSerializer serializer, Predicate<MessageContext> predicate)
    {
        ArgumentNullException.ThrowIfNull(serializer, nameof(serializer));
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        if (_serializers.Any(p => p.Equals(predicate)))
            throw new ArgumentException("Serializer already registered for the given predicate.");
        
        _serializers.Add((predicate, serializer));
    }
}
