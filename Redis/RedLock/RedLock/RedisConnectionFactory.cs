using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace RedLock
{
    public class RedisConnectionFactory
    {
        private static readonly Lazy<ConnectionMultiplexer> Connection;
        private static readonly RedLockFactory _redlockFactory;

        static RedisConnectionFactory()
        {
            var connectionString = "127.0.0.1:6379";
            var options = ConfigurationOptions.Parse(connectionString);
            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public static ConnectionMultiplexer GetConnection() => Connection.Value;

        public static RedLockFactory RedisLockFactory
        {
            get
            {
                var multiplexers = new List<RedLockMultiplexer> { RedisConnectionFactory.GetConnection() };
                return RedLockFactory.Create(multiplexers);
            }
        }
    }
}
