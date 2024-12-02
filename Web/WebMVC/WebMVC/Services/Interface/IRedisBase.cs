using StackExchange.Redis;

namespace WebMVC.Services.Interface
{
    public interface IRedisBase
    {
        ConnectionMultiplexer ConnectionRedis();
    }
}
