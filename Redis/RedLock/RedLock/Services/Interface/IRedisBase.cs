using StackExchange.Redis;

namespace RedLock.Services.Interface
{
    public interface IRedisBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ConnectionMultiplexer ConnectionRedis();
    }
}
