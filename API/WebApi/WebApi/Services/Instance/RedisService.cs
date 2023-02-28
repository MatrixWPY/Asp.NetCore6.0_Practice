using Newtonsoft.Json;
using StackExchange.Redis;
using WebApi.Services.Interface;

namespace WebApi.Services.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisService : IRedisService
    {
        public volatile ConnectionMultiplexer _redisConnection;
        private static object _redisConnectionLock = new object();
        private readonly ConfigurationOptions _configOptions;
        private readonly ILogger<RedisService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public RedisService(ILogger<RedisService> logger, IConfiguration configuration)
        {
            _logger = logger;
            ConfigurationOptions options = ReadRedisSetting(configuration);
            if (options == null)
            {
                _logger.LogError("Redis數據庫配置有誤");
            }

            _configOptions = options;
            _redisConnection = ConnectionRedis();
        }

        private ConfigurationOptions ReadRedisSetting(IConfiguration configuration)
        {
            try
            {
                ConfigurationOptions options = new ConfigurationOptions
                {
                    EndPoints =
                    {
                        {
                            configuration.GetValue<string>("Redis:Ip"),
                            configuration.GetValue<int>("Redis:Port")
                        }
                    },
                    ClientName = configuration.GetValue<string>("Redis:Name"),
                    Password = configuration.GetValue<string>("Redis:Password"),
                    ConnectTimeout = configuration.GetValue<int>("Redis:Timeout"),
                    DefaultDatabase = configuration.GetValue<int>("Redis:Db"),
                };
                return options;
            }
            catch (Exception ex)
            {
                _logger.LogError($"獲取Redis配置信息失敗：{ex.Message}");
                return null;
            }
        }

        private ConnectionMultiplexer ConnectionRedis()
        {
            if (_redisConnection != null && _redisConnection.IsConnected)
            {
                return _redisConnection; // 已有連接，直接使用
            }
            lock (_redisConnectionLock)
            {
                if (_redisConnection != null)
                {
                    _redisConnection.Dispose(); // 釋放，重連
                }
                try
                {
                    _redisConnection = ConnectionMultiplexer.Connect(_configOptions);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Redis服務啟動失敗：{ex.Message}");
                }
            }
            return _redisConnection;
        }

        /// <summary>
        /// 設置緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ts"></param>
        public void Set(string key, object value, TimeSpan ts)
        {
            if (value != null)
            {
                _redisConnection.GetDatabase().StringSet(key, JsonConvert.SerializeObject(value), ts);
            }
        }

        /// <summary>
        /// 獲取緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            return _redisConnection.GetDatabase().StringGet(key);
        }

        /// <summary>
        /// 獲取序列化值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TEntity Get<TEntity>(string key)
        {
            var value = _redisConnection.GetDatabase().StringGet(key);
            if (value.HasValue)
            {
                return JsonConvert.DeserializeObject<TEntity>(value);
            }
            else
            {
                return default(TEntity);
            }
        }

        /// <summary>
        /// 判斷Key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exist(string key)
        {
            return _redisConnection.GetDatabase().KeyExists(key);
        }

        /// <summary>
        /// 移除某個Key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            _redisConnection.GetDatabase().KeyDelete(key);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            foreach (var endPoint in ConnectionRedis().GetEndPoints())
            {
                var server = ConnectionRedis().GetServer(endPoint);
                foreach (var key in server.Keys())
                {
                    _redisConnection.GetDatabase().KeyDelete(key);
                }
            }
        }

        /// <summary>
        /// 異步設置緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public async Task SetAsync(string key, object value, TimeSpan ts)
        {
            if (value != null)
            {
                await _redisConnection.GetDatabase().StringSetAsync(key, JsonConvert.SerializeObject(value), ts);
            }
        }

        /// <summary>
        /// 異步獲取緩存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetValueAsync(string key)
        {
            return await _redisConnection.GetDatabase().StringGetAsync(key);
        }

        /// <summary>
        /// 異步獲取序列化值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<TEntity> GetAsync<TEntity>(string key)
        {
            var value = await _redisConnection.GetDatabase().StringGetAsync(key);
            if (value.HasValue)
            {
                return JsonConvert.DeserializeObject<TEntity>(value);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 異步判斷Key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> ExistAsync(string key)
        {
            return await _redisConnection.GetDatabase().KeyExistsAsync(key);
        }

        /// <summary>
        /// 異步移除某個Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string key)
        {
            await _redisConnection.GetDatabase().KeyDeleteAsync(key);
        }

        /// <summary>
        /// 異步移除模糊查詢到的key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task RemoveByKeyAsync(string key)
        {
            var redisResult = await _redisConnection.GetDatabase().ScriptEvaluateAsync(LuaScript.Prepare(
                //模糊查詢：
                " local res = redis.call('KEYS', @keypattern) " +
                " return res "), new { @keypattern = key + "*" });

            if (!redisResult.IsNull)
            {
                var keys = (string[])redisResult;
                foreach (var k in keys)
                {
                    _redisConnection.GetDatabase().KeyDelete(k);
                }
            }
        }

        /// <summary>
        /// 異步清空
        /// </summary>
        /// <returns></returns>
        public async Task ClearAsync()
        {
            foreach (var endPoint in ConnectionRedis().GetEndPoints())
            {
                var server = ConnectionRedis().GetServer(endPoint);
                foreach (var key in server.Keys())
                {
                    await _redisConnection.GetDatabase().KeyDeleteAsync(key);
                }
            }
        }
    }
}
