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

        #region Common
        /// <summary>
        /// 清空緩存值
        /// </summary>
        public void Clear()
        {
            foreach (var endPoint in ConnectionRedis().GetEndPoints())
            {
                var server = ConnectionRedis().GetServer(endPoint);
                var redisKeys = server.Keys();
                _redisConnection.GetDatabase().KeyDelete(redisKeys.ToArray());
            }
        }

        /// <summary>
        /// 異步清空緩存值
        /// </summary>
        /// <returns></returns>
        public async Task ClearAsync()
        {
            foreach (var endPoint in ConnectionRedis().GetEndPoints())
            {
                var server = ConnectionRedis().GetServer(endPoint);
                var redisKeys = server.Keys();
                await _redisConnection.GetDatabase().KeyDeleteAsync(redisKeys.ToArray());
            }
        }

        /// <summary>
        /// 移除緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        public void Remove(string redisKey)
        {
            _redisConnection.GetDatabase().KeyDelete(redisKey);
        }

        /// <summary>
        /// 異步移除緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string redisKey)
        {
            await _redisConnection.GetDatabase().KeyDeleteAsync(redisKey);
        }

        /// <summary>
        /// 移除緩存值集合
        /// </summary>
        /// <param name="redisKeys"></param>
        public void Remove(IEnumerable<string> redisKeys)
        {
            _redisConnection.GetDatabase().KeyDelete(redisKeys.Select(e => new RedisKey(e)).ToArray());
        }

        /// <summary>
        /// 異步移除緩存值集合
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        public async Task RemoveAsync(IEnumerable<string> redisKeys)
        {
            await _redisConnection.GetDatabase().KeyDeleteAsync(redisKeys.Select(e => new RedisKey(e)).ToArray());
        }

        /// <summary>
        /// 移除模糊查詢到的緩存值
        /// </summary>
        /// <param name="keyPrefix"></param>
        public void RemoveByKey(string keyPrefix)
        {
            var redisResult = _redisConnection.GetDatabase().ScriptEvaluate(
                LuaScript.Prepare(
                    //模糊查詢：
                    " local res = redis.call('KEYS', @keypattern) " + " return res "
                ),
                new { @keypattern = keyPrefix + "*" }
            );

            if (!redisResult.IsNull)
            {
                var redisKeys = (string[])redisResult;
                _redisConnection.GetDatabase().KeyDelete(redisKeys.Select(e => new RedisKey(e)).ToArray());
            }
        }

        /// <summary>
        /// 異步移除模糊查詢到的緩存值
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public async Task RemoveByKeyAsync(string keyPrefix)
        {
            var redisResult = await _redisConnection.GetDatabase().ScriptEvaluateAsync(
                LuaScript.Prepare(
                    //模糊查詢：
                    " local res = redis.call('KEYS', @keypattern) " + " return res "
                ),
                new { @keypattern = keyPrefix + "*" }
            );

            if (!redisResult.IsNull)
            {
                var redisKeys = (string[])redisResult;
                await _redisConnection.GetDatabase().KeyDeleteAsync(redisKeys.Select(e => new RedisKey(e)).ToArray());
            }
        }

        /// <summary>
        /// 判斷緩存值是否存在
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public bool Exist(string redisKey)
        {
            return _redisConnection.GetDatabase().KeyExists(redisKey);
        }

        /// <summary>
        /// 異步判斷緩存值是否存在
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<bool> ExistAsync(string redisKey)
        {
            return await _redisConnection.GetDatabase().KeyExistsAsync(redisKey);
        }
        #endregion

        #region String
        /// <summary>
        /// 獲取String緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public string GetString(string redisKey)
        {
            return _redisConnection.GetDatabase().StringGet(redisKey);
        }

        /// <summary>
        /// 異步獲取String緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string redisKey)
        {
            return await _redisConnection.GetDatabase().StringGetAsync(redisKey);
        }

        /// <summary>
        /// 設置String緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="tsExpiry"></param>
        public void SetString(string redisKey, string redisValue, TimeSpan tsExpiry)
        {
            if (redisValue != null)
            {
                _redisConnection.GetDatabase().StringSet(redisKey, redisValue, tsExpiry);
            }
        }

        /// <summary>
        /// 異步設置String緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="tsExpiry"></param>
        /// <returns></returns>
        public async Task SetStringAsync(string redisKey, string redisValue, TimeSpan tsExpiry)
        {
            if (redisValue != null)
            {
                await _redisConnection.GetDatabase().StringSetAsync(redisKey, redisValue, tsExpiry);
            }
        }

        /// <summary>
        /// 獲取Deserialize緩存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public T GetObject<T>(string redisKey)
        {
            var value = _redisConnection.GetDatabase().StringGet(redisKey);
            if (value.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 異步獲取Deserialize緩存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<T> GetObjectAsync<T>(string redisKey)
        {
            var value = await _redisConnection.GetDatabase().StringGetAsync(redisKey);
            if (value.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 設置Serialize緩存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="tsExpiry"></param>
        public void SetObject<T>(string redisKey, T redisValue, TimeSpan tsExpiry)
        {
            if (redisValue != null)
            {
                _redisConnection.GetDatabase().StringSet(redisKey, JsonConvert.SerializeObject(redisValue), tsExpiry);
            }
        }

        /// <summary>
        /// 異步設置Serialize緩存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="tsExpiry"></param>
        /// <returns></returns>
        public async Task SetObjectAsync<T>(string redisKey, T redisValue, TimeSpan tsExpiry)
        {
            if (redisValue != null)
            {
                await _redisConnection.GetDatabase().StringSetAsync(redisKey, JsonConvert.SerializeObject(redisValue), tsExpiry);
            }
        }
        #endregion

        #region Hash
        /// <summary>
        /// 判斷Hash緩存值是否存在
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public bool ExistHash<TKey>(string redisKey, TKey hashKey)
        {
            return _redisConnection.GetDatabase().HashExists(redisKey, hashKey.ToString());
        }

        /// <summary>
        /// 異步判斷Hash緩存值是否存在
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public async Task<bool> ExistHashAsync<TKey>(string redisKey, TKey hashKey)
        {
            return await _redisConnection.GetDatabase().HashExistsAsync(redisKey, hashKey.ToString());
        }

        /// <summary>
        /// 獲取Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public string GetHash<TKey>(string redisKey, TKey hashKey)
        {
            return _redisConnection.GetDatabase().HashGet(redisKey, hashKey.ToString());
        }

        /// <summary>
        /// 異步獲取Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public async Task<string> GetHashAsync<TKey>(string redisKey, TKey hashKey)
        {
            return await _redisConnection.GetDatabase().HashGetAsync(redisKey, hashKey.ToString());
        }

        /// <summary>
        /// 設置Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <param name="hashValue"></param>
        /// <param name="tsExpiry"></param>
        public void SetHash<TKey>(string redisKey, TKey hashKey, string hashValue, TimeSpan tsExpiry)
        {
            if (hashKey != null && hashValue != null)
            {
                _redisConnection.GetDatabase().HashSet(redisKey, hashKey.ToString(), hashValue);
                _redisConnection.GetDatabase().KeyExpire(redisKey, tsExpiry);
            }
        }

        /// <summary>
        /// 異步設置Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <param name="hashValue"></param>
        /// <param name="tsExpiry"></param>
        /// <returns></returns>
        public async Task SetHashAsync<TKey>(string redisKey, TKey hashKey, string hashValue, TimeSpan tsExpiry)
        {
            if (hashKey != null && hashValue != null)
            {
                await _redisConnection.GetDatabase().HashSetAsync(redisKey, hashKey.ToString(), hashValue);
                await _redisConnection.GetDatabase().KeyExpireAsync(redisKey, tsExpiry);
            }
        }

        /// <summary>
        /// 獲取Hash Deserialize緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public IDictionary<TKey, TValue> GetHashObject<TKey, TValue>(string redisKey)
        {
            return _redisConnection.GetDatabase().HashGetAll(redisKey).ToDictionary(e => (TKey)Convert.ChangeType(e.Name, typeof(TKey)), e => JsonConvert.DeserializeObject<TValue>(e.Value));
        }

        /// <summary>
        /// 異步獲取Hash Deserialize緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<IDictionary<TKey, TValue>> GetHashObjectAsync<TKey, TValue>(string redisKey)
        {
            return (await _redisConnection.GetDatabase().HashGetAllAsync(redisKey)).ToDictionary(e => (TKey)Convert.ChangeType(e.Name, typeof(TKey)), e => JsonConvert.DeserializeObject<TValue>(e.Value));
        }

        /// <summary>
        /// 獲取Hash Deserialize緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public TValue GetHashObject<TKey, TValue>(string redisKey, TKey hashKey)
        {
            return JsonConvert.DeserializeObject<TValue>(_redisConnection.GetDatabase().HashGet(redisKey, hashKey.ToString()));
        }

        /// <summary>
        /// 異步獲取Hash Deserialize緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public async Task<TValue> GetHashObjectAsync<TKey, TValue>(string redisKey, TKey hashKey)
        {
            return JsonConvert.DeserializeObject<TValue>(await _redisConnection.GetDatabase().HashGetAsync(redisKey, hashKey.ToString()));
        }

        /// <summary>
        /// 設置Hash Serialize緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKeyValue"></param>
        /// <param name="tsExpiry"></param>
        public void SetHashObject<TKey, TValue>(string redisKey, IDictionary<TKey, TValue> hashKeyValue, TimeSpan tsExpiry)
        {
            if (hashKeyValue != null)
            {
                _redisConnection.GetDatabase().HashSet(redisKey, hashKeyValue.Select(e => new HashEntry(e.Key.ToString(), JsonConvert.SerializeObject(e.Value))).ToArray());
                _redisConnection.GetDatabase().KeyExpire(redisKey, tsExpiry);
            }
        }

        /// <summary>
        /// 異步設置Hash Serialize緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKeyValue"></param>
        /// <param name="tsExpiry"></param>
        /// <returns></returns>
        public async Task SetHashObjectAsync<TKey, TValue>(string redisKey, IDictionary<TKey, TValue> hashKeyValue, TimeSpan tsExpiry)
        {
            if (hashKeyValue != null)
            {
                await _redisConnection.GetDatabase().HashSetAsync(redisKey, hashKeyValue.Select(e => new HashEntry(e.Key.ToString(), JsonConvert.SerializeObject(e.Value))).ToArray());
                await _redisConnection.GetDatabase().KeyExpireAsync(redisKey, tsExpiry);
            }
        }

        /// <summary>
        /// 移除Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        public void DeleteHash<TKey>(string redisKey, TKey hashKey)
        {
            _redisConnection.GetDatabase().HashDelete(redisKey, hashKey.ToString());
        }

        /// <summary>
        /// 異步移除Hash緩存值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public async Task DeleteHashAsync<TKey>(string redisKey, TKey hashKey)
        {
            await _redisConnection.GetDatabase().HashDeleteAsync(redisKey, hashKey.ToString());
        }

        /// <summary>
        /// 移除Hash緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKeys"></param>
        public void DeleteHash<TKey>(string redisKey, IEnumerable<TKey> hashKeys)
        {
            _redisConnection.GetDatabase().HashDelete(redisKey, hashKeys.Select(e => new RedisValue(e.ToString())).ToArray());
        }

        /// <summary>
        /// 異步移除Hash緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKeys"></param>
        /// <returns></returns>
        public async Task DeleteHashAsync<TKey>(string redisKey, IEnumerable<TKey> hashKeys)
        {
            await _redisConnection.GetDatabase().HashDeleteAsync(redisKey, hashKeys.Select(e => new RedisValue(e.ToString())).ToArray());
        }
        #endregion
    }
}
