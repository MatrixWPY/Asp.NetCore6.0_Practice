using RedLock.Services.Interface;
using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using StackExchange.Redis;

namespace RedLock.Services.Instance
{
    public class RedlockService : IRedlockService
    {
        private readonly ConnectionMultiplexer _redisConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisBase"></param>
        public RedlockService(IRedisBase redisBase)
        {
            _redisConnection = redisBase.ConnectionRedis();
        }

        public RedLockFactory RedisLockFactory
        {
            get
            {
                var multiplexers = new List<RedLockMultiplexer> { _redisConnection };
                return RedLockFactory.Create(multiplexers);
            }
        }
    }
}
