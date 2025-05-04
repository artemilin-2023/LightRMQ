namespace LightRMQ.Abstractions;

public interface IRmqMessageSerializer
{
    string ContentType { get; }
    byte[] Serialize<TObject>(TObject obj);
    TObject Deserialize<TObject>(byte[] data);
}
