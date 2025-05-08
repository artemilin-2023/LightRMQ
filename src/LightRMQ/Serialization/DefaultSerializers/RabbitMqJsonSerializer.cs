using LightRMQ.Abstractions;
using LightRMQ.Common;
using System.Text.Json;

namespace LightRMQ.Serialization.DefaultSerializers;

public class RabbitMqJsonSerializer : IRabbitMqMessageSerializer
{
    public RabbitMqJsonSerializer() { }

    public RabbitMqJsonSerializer(JsonSerializerOptions? options)
    {
        _options = options;
    }

    public string ContentType => ContentTypes.Application.Json;

    private readonly JsonSerializerOptions? _options;

    public TObject Deserialize<TObject>(byte[] data)
        => JsonSerializer.Deserialize<TObject>(data, _options) 
        ?? throw new JsonException("Failed to deserialize the message. Check the serializer configuration and message format.");

    public byte[] Serialize<TObject>(TObject obj)
        => JsonSerializer.SerializeToUtf8Bytes(obj, _options);
}