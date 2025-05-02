using RabbitMQ.Client;

namespace LightRMQ.Abstraction;

internal interface IChannelPool
{
    IChannel GetChannel(IConnection connection);
    void ReturnChannel(IChannel channel);
}
