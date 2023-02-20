using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace BasicTest
{
    public class RedisConnection
    {
        private static ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>> s_connectionPool = new();

        public IDatabase Connect(string setting = "localhost")
        {
            var connMultiplexer = s_connectionPool.GetOrAdd(setting, new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(setting)));

            return connMultiplexer.Value.GetDatabase();
        }

        public IDatabase GetDatabase(string setting = "localhost")
        {
            if (s_connectionPool.TryGetValue(setting, out var connMultiplexer))
            {
                return connMultiplexer.Value.GetDatabase();
            }

            return default;
        }
    }

    public static class RedisExtension
    {
        private static string Serialize<T>(T value, JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize(value, options);
        }

        private static T? Deserialize<T>(RedisValue value, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(value, options);
        }

        public static bool IsExist(this IDatabase db, string key)
        {
            return db.KeyExists(key);
        }

        public static T Get<T>(this IDatabase db, string key, JsonSerializerOptions options = default)
        {
            if (db.IsExist(key))
            {
                return Deserialize<T>(db.StringGet(key), options);
            }

            return default;
        }

        public static void Set<T>(this IDatabase db, string key, T value,
            TimeSpan? expiry = default,
            When when = When.Always,
            CommandFlags flags = CommandFlags.None,
            JsonSerializerOptions options = default)
        {
            db.StringSet(key, Serialize(value, options), expiry, when, flags);
        }
    }
}
