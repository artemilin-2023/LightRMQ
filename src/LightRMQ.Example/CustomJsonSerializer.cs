using System.Text.Json;

namespace LightRMQ.Example;

public class CustomJsonSerializer(ILogger<CustomJsonSerializer> logger) : IRabbitMqMessageSerializer
{
    public string ContentType => ContentTypes.Application.Json;

    public string? ContentEncoding => "utf-8";

    public TObject Deserialize<TObject>(byte[] data)
    {
        //logger.LogInformation("Using custom json serializer!");

        return JsonSerializer.Deserialize<TObject>(data) ?? throw new InvalidOperationException("Deserialization failed.");
    }

    public byte[] Serialize<TObject>(TObject obj)
    {
        //logger.LogInformation("Using custom json serializer!");
        
        return JsonSerializer.SerializeToUtf8Bytes(obj);
    }
}
