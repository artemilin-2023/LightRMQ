namespace LightRMQ.Abstractions;

public interface IRabbitMqMessageSerializer
{
    string ContentType { get; }
    byte[] Serialize<TObject>(TObject obj);
    TObject Deserialize<TObject>(byte[] data);
}
