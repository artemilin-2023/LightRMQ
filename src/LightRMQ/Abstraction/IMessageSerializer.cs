namespace LightRMQ.Abstraction;

public interface IMessageSerializer
{
    string ContentType { get; }
    byte[] Serialize<TObject>(TObject obj);
    TObject Deserialize<TObject>(byte[] data);
}
