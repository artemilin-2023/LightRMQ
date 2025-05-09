using System.Text.Json;

namespace LightRMQ.Serialization.DefaultSerializers;

internal class RabbitMqJsonSerializer : 
    IRabbitMqMessageSerializer
{
    public string ContentType => ContentTypes.Application.Json;
    public string ContentEncoding => "utf-8";

    private readonly JsonSerializerOptions? _options;

    public RabbitMqJsonSerializer() { }

    public RabbitMqJsonSerializer(JsonSerializerOptions? options)
    {
        _options = options;
    }

    public TObject Deserialize<TObject>(byte[] data)
        => JsonSerializer.Deserialize<TObject>(data, _options) 
        ?? throw new JsonException("Failed to deserialize the message. Check the serializer configuration and message format.");

    public byte[] Serialize<TObject>(TObject obj)
        => JsonSerializer.SerializeToUtf8Bytes(obj, _options);
}