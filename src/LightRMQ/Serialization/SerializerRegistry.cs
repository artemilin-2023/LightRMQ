using LightRMQ.Core;
using LightRMQ.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace LightRMQ.Serialization;

internal class SerializerRegistry(ILogger<SerializerRegistry> logger) : 
    ISerializerRegistry
{
    public IRabbitMqMessageSerializer DefualtSerializer
    {
        get => _defualtSerializer ?? throw new Exception("No default serializer set. Ensure that a default serializer is configured.");
        set => _defualtSerializer ??= value;
    }

    private readonly List<(Predicate<ContextArgs> predicate, IRabbitMqMessageSerializer serializer)> _serializers = [];
    private readonly ConcurrentDictionary<Type, IRabbitMqMessageSerializer> _messageTypeSerializersCache = [];
    private readonly ConcurrentDictionary<Type, IRabbitMqMessageSerializer> _serializersTypeCache = [];
    private IRabbitMqMessageSerializer? _defualtSerializer;
    private readonly ILogger<SerializerRegistry> _logger = logger;

    public IRabbitMqMessageSerializer GetByContextOrDefault(ContextArgs context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        if (context.TargetMessageType is not null && _messageTypeSerializersCache.TryGetValue(context.TargetMessageType, out var cachedSerializer))
        {
            _logger.LogDebug("Using cached serializer {Serializer} for message type: {MessageType}", cachedSerializer.GetType().Name, context.TargetMessageType.Name);
            return cachedSerializer;
        }

        _logger.LogDebug("No cached serializer found for message type: {MessageType}.", context.TargetMessageType?.Name);
        foreach (var (predicate, serializer) in _serializers)
        {
            if (predicate(context))
            {
                if (context.TargetMessageType is not null)
                    _messageTypeSerializersCache.TryAdd(context.TargetMessageType, serializer);

                _logger.LogDebug("Using serializer {Serializer} for message type: {MessageType}", serializer.GetType().Name, context.TargetMessageType!.Name);
                return serializer;
            }
        }

        _logger.LogDebug(_defualtSerializer is null ? "No matching serializer found and no default serializer set." : "No matching serializer found, using default serializer.");
        return _defualtSerializer ?? throw new Exception("No serializer found for the given context. Ensure that serializers are configured or set a default one.");
    }

    public void RegisterSerializer(IRabbitMqMessageSerializer serializer, Predicate<ContextArgs> predicate)
    {
        ArgumentNullException.ThrowIfNull(serializer, nameof(serializer));
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        if (_serializers.Any(p => p.Equals(predicate)))
            throw new ArgumentException("Serializer already registered for the given predicate.");
        
        _serializers.Add((predicate, serializer));
    }

    public IRabbitMqMessageSerializer? Get(Type serializerType)
    {
        if (serializerType.GetInterface(nameof(IRabbitMqMessageSerializer)) is null)
            throw new InvalidOperationException($"Type {serializerType.Name} is not a valid serializer type.");
        
        if (_serializersTypeCache.TryGetValue(serializerType, out var cachedSerializer))
        {
            _logger.LogDebug("Using cached serializer '{Serializer}'", serializerType.Name);
            return cachedSerializer;
        }

        var serializer = _serializers!.FirstOrDefault(i => i.serializer!.GetType() == serializerType, defaultValue: (null, null));
        if (serializer is (null, null))
            return null;

        _serializersTypeCache.TryAdd(serializerType, serializer.serializer!);
        _logger.LogDebug("Added serializer '{Serializer}' to cache", serializerType.Name);
        return serializer.serializer!;
    }

    public IRabbitMqMessageSerializer GetOrDefault(Type? serializerType)
        => serializerType is not null 
        ? Get(serializerType) ?? DefualtSerializer
        : DefualtSerializer;
}