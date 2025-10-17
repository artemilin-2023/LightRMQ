namespace LightRMQ;

public interface IRabbitMqMessageSerializer
{
    string ContentType { get; }
    string? ContentEncoding { get; }
    byte[] Serialize<TObject>(TObject obj);
    TObject Deserialize<TObject>(byte[] data);
}
