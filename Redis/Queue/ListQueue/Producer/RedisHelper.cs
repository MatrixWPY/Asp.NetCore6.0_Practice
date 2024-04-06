using StackExchange.Redis;

namespace Producer
{
    public class RedisHelper
    {
        public sealed class RedisConnection
        {
            private static string _settingOption;

            private static Lazy<RedisConnection> lazy = new Lazy<RedisConnection>(() =>
            {
                if (String.IsNullOrEmpty(_settingOption))
                {
                    throw new InvalidOperationException("Please call Init() first.");
                }
                return new RedisConnection();
            });

            private RedisConnection()
            {
                ConnectionMultiplexer = ConnectionMultiplexer.Connect(_settingOption);
            }

            public readonly ConnectionMultiplexer ConnectionMultiplexer;

            public static RedisConnection Instance
            {
                get
                {
                    return lazy.Value;
                }
            }

            public static void Init(string settingOption)
            {
                _settingOption = settingOption;
            }
        }
    }
}
