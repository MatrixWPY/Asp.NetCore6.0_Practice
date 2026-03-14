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
        private readonly ILogger<RedisService> _logger;
        private readonly ConnectionMultiplexer _connection;
        private readonly IDatabase _db;
        private const string _redisEmptyValue = "EMPTY_VALUE";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="redisBaseService"></param>
        public RedisService(ILogger<RedisService> logger, RedisBaseService redisBaseService)
        {
            _logger = logger;
            _connection = redisBaseService.Connection;
            _db = _connection.GetDatabase();
        }

        #region Common
        /// <summary>
        /// 清空緩存值
        /// </summary>
        public void Clear()
        {
            try
            {
                var endpoints = _connection.GetEndPoints();

                foreach (var endpoint in endpoints)
                {
                    var server = _connection.GetServer(endpoint);
                    if (!server.IsReplica)
                    {
                        server.FlushDatabase(_db.Database);
                        _logger.LogInformation($"已清空 Redis 節點 {endpoint} 的 Database {_db.Database}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "清空 Redis 快取時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 異步清空緩存值
        /// </summary>
        /// <returns></returns>
        public async Task ClearAsync()
        {
            try
            {
                var endpoints = _connection.GetEndPoints();
                var flushTasks = new List<Task>();

                foreach (var endpoint in endpoints)
                {
                    var server = _connection.GetServer(endpoint);
                    if (!server.IsReplica)
                    {
                        flushTasks.Add(server.FlushDatabaseAsync(_db.Database));
                        _logger.LogInformation($"準備清空 Redis 節點 {endpoint} 的 Database {_db.Database}");
                    }
                }

                await Task.WhenAll(flushTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "非同步清空 Redis 快取時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 移除緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        public void Remove(string redisKey)
        {
            _db.KeyDelete(redisKey);
        }

        /// <summary>
        /// 異步移除緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string redisKey)
        {
            await _db.KeyDeleteAsync(redisKey);
        }

        /// <summary>
        /// 移除緩存值集合
        /// </summary>
        /// <param name="redisKeys"></param>
        public void Remove(IEnumerable<string> redisKeys)
        {
            _db.KeyDelete(redisKeys.Select(e => new RedisKey(e)).ToArray());
        }

        /// <summary>
        /// 異步移除緩存值集合
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        public async Task RemoveAsync(IEnumerable<string> redisKeys)
        {
            await _db.KeyDeleteAsync(redisKeys.Select(e => new RedisKey(e)).ToArray());
        }

        /// <summary>
        /// 移除模糊查詢到的緩存值
        /// </summary>
        /// <param name="keyPrefix"></param>
        public void RemoveByKey(string keyPrefix)
        {
            var endpoints = _connection.GetEndPoints();

            foreach (var endpoint in endpoints)
            {
                var server = _connection.GetServer(endpoint);
                if (!server.IsReplica)
                {
                    var keys = server.Keys(database: _db.Database, pattern: $"{keyPrefix}*").ToArray();

                    if (keys.Length > 0)
                    {
                        _db.KeyDelete(keys);
                    }
                }
            }
        }

        /// <summary>
        /// 異步移除模糊查詢到的緩存值
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public async Task RemoveByKeyAsync(string keyPrefix)
        {
            var endpoints = _connection.GetEndPoints();
            var deleteTasks = new List<Task>();

            foreach (var endpoint in endpoints)
            {
                var server = _connection.GetServer(endpoint);
                if (!server.IsReplica)
                {
                    var keys = server.Keys(database: _db.Database, pattern: $"{keyPrefix}*").ToArray();

                    if (keys.Length > 0)
                    {
                        deleteTasks.Add(_db.KeyDeleteAsync(keys));
                    }
                }
            }

            if (deleteTasks.Count > 0)
            {
                await Task.WhenAll(deleteTasks);
            }
        }

        /// <summary>
        /// 判斷緩存值是否存在
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public bool Exist(string redisKey)
        {
            return _db.KeyExists(redisKey);
        }

        /// <summary>
        /// 異步判斷緩存值是否存在
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<bool> ExistAsync(string redisKey)
        {
            return await _db.KeyExistsAsync(redisKey);
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
            return _db.StringGet(redisKey);
        }

        /// <summary>
        /// 異步獲取String緩存值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string redisKey)
        {
            return await _db.StringGetAsync(redisKey);
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
                _db.StringSet(redisKey, redisValue, tsExpiry);
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
                await _db.StringSetAsync(redisKey, redisValue, tsExpiry);
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
            var value = _db.StringGet(redisKey);
            if (value.HasValue && value != _redisEmptyValue)
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
            var value = await _db.StringGetAsync(redisKey);
            if (value.HasValue && value != _redisEmptyValue)
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
                _db.StringSet(redisKey, JsonConvert.SerializeObject(redisValue), tsExpiry);
            }
            else
            {
                _db.StringSet(redisKey, _redisEmptyValue, tsExpiry);
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
                await _db.StringSetAsync(redisKey, JsonConvert.SerializeObject(redisValue), tsExpiry);
            }
            else
            {
                await _db.StringSetAsync(redisKey, _redisEmptyValue, tsExpiry);
            }
        }

        /// <summary>
        /// 設置Serialize緩存值，並加上隨機過期時間
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="tsBaseExpiry"></param>
        /// <param name="tsMaxJitter"></param>
        public void SetObjectWithJitter<T>(string redisKey, T redisValue, TimeSpan tsBaseExpiry, TimeSpan tsMaxJitter)
        {
            if (redisValue != null)
            {
                string jsonValue = JsonConvert.SerializeObject(redisValue);

                double randomMs = Random.Shared.NextDouble() * tsMaxJitter.TotalMilliseconds;
                TimeSpan jitter = TimeSpan.FromMilliseconds(randomMs);
                TimeSpan finalExpiry = tsBaseExpiry + jitter;

                _db.StringSet(redisKey, jsonValue, finalExpiry);
            }
        }

        /// <summary>
        /// 異步設置Serialize緩存值，並加上隨機過期時間
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="tsBaseExpiry"></param>
        /// <param name="tsMaxJitter"></param>
        /// <returns></returns>
        public async Task SetObjectWithJitterAsync<T>(string redisKey, T redisValue, TimeSpan tsBaseExpiry, TimeSpan tsMaxJitter)
        {
            if (redisValue != null)
            {
                string jsonValue = JsonConvert.SerializeObject(redisValue);

                double randomMs = Random.Shared.NextDouble() * tsMaxJitter.TotalMilliseconds;
                TimeSpan jitter = TimeSpan.FromMilliseconds(randomMs);
                TimeSpan finalExpiry = tsBaseExpiry + jitter;

                await _db.StringSetAsync(redisKey, jsonValue, finalExpiry);
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
            return _db.HashExists(redisKey, hashKey.ToString());
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
            return await _db.HashExistsAsync(redisKey, hashKey.ToString());
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
            return _db.HashGet(redisKey, hashKey.ToString());
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
            return await _db.HashGetAsync(redisKey, hashKey.ToString());
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
                _db.HashSet(redisKey, hashKey.ToString(), hashValue);
                _db.KeyExpire(redisKey, tsExpiry);
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
                await _db.HashSetAsync(redisKey, hashKey.ToString(), hashValue);
                await _db.KeyExpireAsync(redisKey, tsExpiry);
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
            return _db.HashGetAll(redisKey).ToDictionary(e => (TKey)Convert.ChangeType(e.Name, typeof(TKey)), e => JsonConvert.DeserializeObject<TValue>(e.Value));
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
            return (await _db.HashGetAllAsync(redisKey)).ToDictionary(e => (TKey)Convert.ChangeType(e.Name, typeof(TKey)), e => JsonConvert.DeserializeObject<TValue>(e.Value));
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
            var hashValue = _db.HashGet(redisKey, hashKey.ToString());
            if (hashValue != _redisEmptyValue)
            {
                return JsonConvert.DeserializeObject<TValue>(hashValue);
            }
            else
            {
                return default;
            }
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
            var hashValue = await _db.HashGetAsync(redisKey, hashKey.ToString());
            if (hashValue != _redisEmptyValue)
            {
                return JsonConvert.DeserializeObject<TValue>(hashValue);
            }
            else
            {
                return default;
            }
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
            if (hashKeyValue != null && hashKeyValue.Count > 0)
            {
                HashEntry[] hashFields = hashKeyValue.Select(e =>
                    new HashEntry(e.Key.ToString(), e.Value == null ? _redisEmptyValue : JsonConvert.SerializeObject(e.Value))
                ).ToArray();

                var batch = _db.CreateBatch();
                batch.HashSetAsync(redisKey, hashFields);
                batch.KeyExpireAsync(redisKey, tsExpiry);
                batch.Execute();
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
            if (hashKeyValue != null && hashKeyValue.Count > 0)
            {
                HashEntry[] hashFields = hashKeyValue.Select(e =>
                    new HashEntry(e.Key.ToString(), e.Value == null ? _redisEmptyValue : JsonConvert.SerializeObject(e.Value))
                ).ToArray();

                var batch = _db.CreateBatch();
                Task taskSet = batch.HashSetAsync(redisKey, hashFields);
                Task taskExpire = batch.KeyExpireAsync(redisKey, tsExpiry);
                batch.Execute();

                await Task.WhenAll(taskSet, taskExpire);
            }
        }

        /// <summary>
        /// 設置Hash Serialize緩存值，並加上隨機過期時間
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKeyValue"></param>
        /// <param name="tsBaseExpiry"></param>
        /// <param name="tsMaxJitter"></param>
        public void SetHashObjectWithJitter<TKey, TValue>(string redisKey, IDictionary<TKey, TValue> hashKeyValue, TimeSpan tsBaseExpiry, TimeSpan tsMaxJitter)
        {
            if (hashKeyValue != null && hashKeyValue.Count > 0)
            {
                HashEntry[] hashFields = hashKeyValue.Select(e => new HashEntry(e.Key.ToString(), JsonConvert.SerializeObject(e.Value))).ToArray();

                double randomMs = Random.Shared.NextDouble() * tsMaxJitter.TotalMilliseconds;
                TimeSpan jitter = TimeSpan.FromMilliseconds(randomMs);
                TimeSpan finalExpiry = tsBaseExpiry + jitter;

                var batch = _db.CreateBatch();
                batch.HashSetAsync(redisKey, hashFields);
                batch.KeyExpireAsync(redisKey, finalExpiry);
                batch.Execute();
            }
        }

        /// <summary>
        /// 異步設置Hash Serialize緩存值，並加上隨機過期時間
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKeyValue"></param>
        /// <param name="tsBaseExpiry"></param>
        /// <param name="tsMaxJitter"></param>
        /// <returns></returns>
        public async Task SetHashObjectWithJitterAsync<TKey, TValue>(string redisKey, IDictionary<TKey, TValue> hashKeyValue, TimeSpan tsBaseExpiry, TimeSpan tsMaxJitter)
        {
            if (hashKeyValue != null && hashKeyValue.Count > 0)
            {
                HashEntry[] hashFields = hashKeyValue.Select(e => new HashEntry(e.Key.ToString(), JsonConvert.SerializeObject(e.Value))).ToArray();

                double randomMs = Random.Shared.NextDouble() * tsMaxJitter.TotalMilliseconds;
                TimeSpan jitter = TimeSpan.FromMilliseconds(randomMs);
                TimeSpan finalExpiry = tsBaseExpiry + jitter;

                var batch = _db.CreateBatch();
                Task taskSet = batch.HashSetAsync(redisKey, hashFields);
                Task taskExpire = batch.KeyExpireAsync(redisKey, finalExpiry);
                batch.Execute();

                await Task.WhenAll(taskSet, taskExpire);
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
            _db.HashDelete(redisKey, hashKey.ToString());
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
            await _db.HashDeleteAsync(redisKey, hashKey.ToString());
        }

        /// <summary>
        /// 移除Hash緩存值集合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="hashKeys"></param>
        public void DeleteHash<TKey>(string redisKey, IEnumerable<TKey> hashKeys)
        {
            _db.HashDelete(redisKey, hashKeys.Select(e => new RedisValue(e.ToString())).ToArray());
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
            await _db.HashDeleteAsync(redisKey, hashKeys.Select(e => new RedisValue(e.ToString())).ToArray());
        }
        #endregion

        #region ListQueue
        /// <summary>
        /// 接收資料從 List Queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public IEnumerable<T> ReceiveListQueue<T>(string queueName, int maxCount)
        {
            List<T> res = new List<T>();

            var listDatas = _db.ListRightPop(queueName, maxCount);
            if (listDatas?.Any() ?? false)
            {
                foreach (var data in listDatas)
                {
                    res.Add(JsonConvert.DeserializeObject<T>(data));
                }
            }

            return res;
        }

        /// <summary>
        /// 異步接收資料從 List Queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ReceiveListQueueAsync<T>(string queueName, int maxCount)
        {
            List<T> res = new List<T>();

            var listDatas = await _db.ListRightPopAsync(queueName, maxCount);
            if (listDatas?.Any() ?? false)
            {
                foreach (var data in listDatas)
                {
                    res.Add(JsonConvert.DeserializeObject<T>(data));
                }
            }

            return res;
        }

        /// <summary>
        /// 傳送資料至 List Queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="redisValue"></param>
        public void SendListQueue<T>(string queueName, T redisValue)
        {
            _db.ListLeftPush(queueName, JsonConvert.SerializeObject(redisValue));
        }

        /// <summary>
        /// 異步傳送資料至 List Queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task SendListQueueAsync<T>(string queueName, T redisValue)
        {
            await _db.ListLeftPushAsync(queueName, JsonConvert.SerializeObject(redisValue));
        }
        #endregion

        #region StreamQueue
        /// <summary>
        /// 接收資料從 Stream Queue
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="groupName"></param>
        /// <param name="consumerName"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public IDictionary<TKey, TValue> ReceiveStreamQueue<TKey, TValue>(string queueName, string groupName, string consumerName, int maxCount)
        {
            IDictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();

            #region 前置判斷
            var hasKey = _db.KeyExists(queueName, CommandFlags.None);
            if (hasKey == false)
            {
                _logger.LogError($"不存在 Queue = {queueName}");
                return res;
            }

            var groups = _db.StreamGroupInfo(queueName, CommandFlags.None);
            if (groups == null || groups?.Any(x => x.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)) == false)
            {
                _logger.LogError($"不存在 Group = {groupName}");
                return res;
            }
            #endregion

            var streamDatas = _db.StreamReadGroup(queueName, groupName, consumerName, ">", maxCount, noAck: false);
            if (streamDatas.Any())
            {
                foreach (var streamData in streamDatas)
                {
                    var id = streamData.Id;
                    foreach (var data in streamData.Values)
                    {
                        res[(TKey)Convert.ChangeType(data.Name, typeof(TKey))] = JsonConvert.DeserializeObject<TValue>(data.Value);
                    }

                    _db.StreamAcknowledge(queueName, groupName, id);
                    _db.StreamDelete(queueName, new RedisValue[] { id });
                }
            }

            return res;
        }

        /// <summary>
        /// 異步接收資料從 Stream Queue
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="groupName"></param>
        /// <param name="consumerName"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public async Task<IDictionary<TKey, TValue>> ReceiveStreamQueueAsync<TKey, TValue>(string queueName, string groupName, string consumerName, int maxCount)
        {
            IDictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();

            #region 前置判斷
            var hasKey = await _db.KeyExistsAsync(queueName, CommandFlags.None);
            if (hasKey == false)
            {
                _logger.LogError($"不存在 Queue = {queueName}");
                return res;
            }

            var groups = await _db.StreamGroupInfoAsync(queueName, CommandFlags.None);
            if (groups == null || groups?.Any(x => x.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)) == false)
            {
                _logger.LogError($"不存在 Group = {groupName}");
                return res;
            }
            #endregion

            var streamDatas = await _db.StreamReadGroupAsync(queueName, groupName, consumerName, ">", maxCount, noAck: false);
            if (streamDatas.Any())
            {
                foreach (var streamData in streamDatas)
                {
                    var id = streamData.Id;
                    foreach (var data in streamData.Values)
                    {
                        res[(TKey)Convert.ChangeType(data.Name, typeof(TKey))] = JsonConvert.DeserializeObject<TValue>(data.Value);
                    }

                    await _db.StreamAcknowledgeAsync(queueName, groupName, id);
                    await _db.StreamDeleteAsync(queueName, new RedisValue[] { id });
                }
            }

            return res;
        }

        /// <summary>
        /// 傳送資料至 Stream Queue
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="groupName"></param>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        public void SendStreamQueue<TKey, TValue>(string queueName, string groupName, TKey redisKey, TValue redisValue)
        {
            #region 前置設定 Consumer Group
            // 因需先 StreamCreateConsumerGroup 後再 StreamAdd，後續 StreamReadGroup 才讀的到資料
            // 故在 Producer 時先行設置 Consumer 會使用的 GroupName

            var hasKey = _db.KeyExists(queueName, CommandFlags.None);
            if (hasKey == false)
            {
                // 此時尚未執行 StreamCreateConsumerGroup，故此筆資料不會被 StreamReadGroup 讀出
                // 此處執行 StreamAdd 目的為使之後的 StreamGroupInfo 可作用
                _db.StreamAdd(queueName, "-1", $"Root_{queueName}");
            }

            var groups = _db.StreamGroupInfo(queueName, CommandFlags.None);
            if (groups == null || groups?.Any(x => x.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)) == false)
            {
                try
                {
                    // 設置 Consumer 會使用的 GroupName
                    // 之後執行 StreamAdd 的資料已可被 StreamReadGroup 讀出
                    _db.StreamCreateConsumerGroup(queueName, groupName, StreamPosition.NewMessages);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"建立 Group 失敗 : {ex.Message}");
                    throw;
                }
            }
            #endregion

            _db.StreamAdd(queueName, redisKey.ToString(), JsonConvert.SerializeObject(redisValue));
        }

        /// <summary>
        /// 異步傳送資料至 Stream Queue
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="groupName"></param>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task SendStreamQueueAsync<TKey, TValue>(string queueName, string groupName, TKey redisKey, TValue redisValue)
        {
            #region 前置設定 Consumer Group
            // 因需先 StreamCreateConsumerGroupAsync 後再 StreamAddAsync，後續 StreamReadGroupAsync 才讀的到資料
            // 故在 Producer 時先行設置 Consumer 會使用的 GroupName

            var hasKey = await _db.KeyExistsAsync(queueName, CommandFlags.None);
            if (hasKey == false)
            {
                // 此時尚未執行 StreamCreateConsumerGroupAsync，故此筆資料不會被 StreamReadGroupAsync 讀出
                // 此處執行 StreamAddAsync 目的為使之後的 StreamGroupInfoAsync 可作用
                await _db.StreamAddAsync(queueName, "-1", $"Root_{queueName}");
            }

            var groups = await _db.StreamGroupInfoAsync(queueName, CommandFlags.None);
            if (groups == null || groups?.Any(x => x.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)) == false)
            {
                try
                {
                    // 設置 Consumer 會使用的 GroupName
                    // 之後執行 StreamAddAsync 的資料已可被 StreamReadGroupAsync 讀出
                    await _db.StreamCreateConsumerGroupAsync(queueName, groupName, StreamPosition.NewMessages);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"建立 Group 失敗 : {ex.Message}");
                    throw;
                }
            }
            #endregion

            await _db.StreamAddAsync(queueName, redisKey.ToString(), JsonConvert.SerializeObject(redisValue));
        }
        #endregion

        #region Publish/Subscribe ListQueue
        /// <summary>
        /// 訂閱 Channel 通知
        /// 接收資料從 List Queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="action"></param>
        public void SubscribeListQueue<T>(string channelName, Action<T> action)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                return;
            }

            var sub = _connection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            sub.Subscribe(channel, (chl, msg) =>
            {
                Task.Run(() =>
                {
                    string queueName = msg;
                    if (queueName.EndsWith($":{typeof(T).Name}") == false)
                    {
                        return;
                    }

                    while (true)
                    {
                        var data = _db.ListRightPop(queueName);
                        if (data.IsNullOrEmpty)
                        {
                            break;
                        }

                        action(JsonConvert.DeserializeObject<T>(data));
                    }
                });
            });
        }

        /// <summary>
        /// 異步
        /// 訂閱 Channel 通知
        /// 接收資料從 List Queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task SubscribeListQueueAsync<T>(string channelName, Func<T, Task> func)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                return;
            }

            var sub = _connection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            await sub.SubscribeAsync(channel, (chl, msg) =>
            {
                Task.Run(async () =>
                {
                    string queueName = msg;
                    if (queueName.EndsWith($":{typeof(T).Name}") == false)
                    {
                        return;
                    }

                    while (true)
                    {
                        var data = await _db.ListRightPopAsync(queueName);
                        if (data.IsNullOrEmpty)
                        {
                            break;
                        }

                        await func(JsonConvert.DeserializeObject<T>(data));
                    }
                });
            });
        }

        /// <summary>
        /// 訂閱 Channel 通知
        /// 接收多筆資料從 List Queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="action"></param>
        public void SubscribeListQueue<T>(string channelName, Action<IEnumerable<T>> action)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                return;
            }

            var sub = _connection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            sub.Subscribe(channel, (chl, msg) =>
            {
                Task.Run(() =>
                {
                    string queueName = msg;
                    if (queueName.EndsWith($":{typeof(T).Name}") == false)
                    {
                        return;
                    }

                    var datas = _db.ListRightPop(queueName, _db.ListLength(queueName));
                    if (datas == null)
                    {
                        return;
                    }

                    action(datas.Select(e => JsonConvert.DeserializeObject<T>(e)));
                });
            });
        }

        /// <summary>
        /// 異步
        /// 訂閱 Channel 通知
        /// 接收多筆資料從 List Queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task SubscribeListQueueAsync<T>(string channelName, Func<IEnumerable<T>, Task> func)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                return;
            }

            var sub = _connection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            await sub.SubscribeAsync(channel, (chl, msg) =>
            {
                Task.Run(async () =>
                {
                    string queueName = msg;
                    if (queueName.EndsWith($":{typeof(T).Name}") == false)
                    {
                        return;
                    }

                    var datas = await _db.ListRightPopAsync(queueName, await _db.ListLengthAsync(queueName));
                    if (datas == null)
                    {
                        return;
                    }

                    await func(datas.Select(e => JsonConvert.DeserializeObject<T>(e)));
                });
            });
        }

        /// <summary>
        /// 訂閱 Channel 通知
        /// 接收資料從 List Queue (Simulate Ack)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="func"></param>
        public void SubscribeListQueue<T>(string channelName, Func<T, bool> func)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                return;
            }

            var sub = _connection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            sub.Subscribe(channel, (chl, msg) =>
            {
                Task.Run(() =>
                {
                    string queueName = msg;
                    if (queueName.EndsWith($":{typeof(T).Name}") == false)
                    {
                        return;
                    }

                    while (true)
                    {
                        var data = _db.ListRightPop(queueName);
                        if (data.IsNullOrEmpty)
                        {
                            break;
                        }

                        var res = func(JsonConvert.DeserializeObject<T>(data));
                        if (res == false)
                        {
                            _db.ListRightPush(queueName, data);
                        }
                    }
                });
            });
        }

        /// <summary>
        /// 異步
        /// 訂閱 Channel 通知
        /// 接收資料從 List Queue (Simulate Ack)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="funcAsync"></param>
        /// <returns></returns>
        public async Task SubscribeListQueueAsync<T>(string channelName, Func<T, Task<bool>> funcAsync)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                return;
            }

            var sub = _connection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            await sub.SubscribeAsync(channel, (chl, msg) =>
            {
                Task.Run(async () =>
                {
                    string queueName = msg;
                    if (queueName.EndsWith($":{typeof(T).Name}") == false)
                    {
                        return;
                    }

                    while (true)
                    {
                        var data = await _db.ListRightPopAsync(queueName);
                        if (data.IsNullOrEmpty)
                        {
                            break;
                        }

                        var res = await funcAsync(JsonConvert.DeserializeObject<T>(data));
                        if (res == false)
                        {
                            await _db.ListRightPushAsync(queueName, data);
                        }
                    }
                });
            });
        }

        /// <summary>
        /// 傳送資料至 List Queue
        /// 發佈 Channel 通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="data"></param>
        public void PublishListQueue<T>(string channelName, T data)
        {
            if (string.IsNullOrWhiteSpace(channelName) || data == null)
            {
                return;
            }

            string queueName = $"{channelName}:{typeof(T).Name}";
            _db.ListLeftPush(queueName, JsonConvert.SerializeObject(data));
            _db.Publish(channelName, queueName);
        }

        /// <summary>
        /// 異步
        /// 傳送資料至 List Queue
        /// 發佈 Channel 通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task PublishListQueueAsync<T>(string channelName, T data)
        {
            if (string.IsNullOrWhiteSpace(channelName) || data == null)
            {
                return;
            }

            string queueName = $"{channelName}:{typeof(T).Name}";
            await _db.ListLeftPushAsync(queueName, JsonConvert.SerializeObject(data));
            await _db.PublishAsync(channelName, queueName);
        }

        /// <summary>
        /// 傳送多筆資料至 List Queue
        /// 發佈 Channel 通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="datas"></param>
        public void PublishListQueue<T>(string channelName, IEnumerable<T> datas)
        {
            if (string.IsNullOrWhiteSpace(channelName) || datas == null)
            {
                return;
            }

            string queueName = $"{channelName}:{typeof(T).Name}";
            _db.ListLeftPush(queueName, datas.Select(e => new RedisValue(JsonConvert.SerializeObject(e))).ToArray());
            _db.Publish(channelName, queueName);
        }

        /// <summary>
        /// 異步
        /// 傳送多筆資料至 List Queue
        /// 發佈 Channel 通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public async Task PublishListQueueAsync<T>(string channelName, IEnumerable<T> datas)
        {
            if (string.IsNullOrWhiteSpace(channelName) || datas == null)
            {
                return;
            }

            string queueName = $"{channelName}:{typeof(T).Name}";
            await _db.ListLeftPushAsync(queueName, datas.Select(e => new RedisValue(JsonConvert.SerializeObject(e))).ToArray());
            await _db.PublishAsync(channelName, queueName);
        }

        /// <summary>
        /// 傳送資料至多個 List Queue
        /// 發佈多個 Channel 通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelNames"></param>
        /// <param name="data"></param>
        public void PublishListQueue<T>(IEnumerable<string> channelNames, T data)
        {
            if (channelNames == null || channelNames.Any() == false || data == null)
            {
                return;
            }

            foreach (var channelName in channelNames)
            {
                string queueName = $"{channelName}:{typeof(T).Name}";
                _db.ListLeftPush(queueName, JsonConvert.SerializeObject(data));
                _db.Publish(channelName, queueName);
            }
        }

        /// <summary>
        /// 異步
        /// 傳送資料至多個 List Queue
        /// 發佈多個 Channel 通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelNames"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task PublishListQueueAsync<T>(IEnumerable<string> channelNames, T data)
        {
            if (channelNames == null || channelNames.Any() == false || data == null)
            {
                return;
            }

            foreach (var channelName in channelNames)
            {
                string queueName = $"{channelName}:{typeof(T).Name}";
                await _db.ListLeftPushAsync(queueName, JsonConvert.SerializeObject(data));
                await _db.PublishAsync(channelName, queueName);
            }
        }

        /// <summary>
        /// 傳送多筆資料至多個 List Queue
        /// 發佈多個 Channel 通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelNames"></param>
        /// <param name="datas"></param>
        public void PublishListQueue<T>(IEnumerable<string> channelNames, IEnumerable<T> datas)
        {
            if (channelNames == null || channelNames.Any() == false || datas == null)
            {
                return;
            }

            foreach (var channelName in channelNames)
            {
                string queueName = $"{channelName}:{typeof(T).Name}";
                _db.ListLeftPush(queueName, datas.Select(e => new RedisValue(JsonConvert.SerializeObject(e))).ToArray());
                _db.Publish(channelName, queueName);
            }
        }

        /// <summary>
        /// 異步
        /// 傳送多筆資料至多個 List Queue
        /// 發佈多個 Channel 通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelNames"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public async Task PublishListQueueAsync<T>(IEnumerable<string> channelNames, IEnumerable<T> datas)
        {
            if (channelNames == null || channelNames.Any() == false || datas == null)
            {
                return;
            }

            foreach (var channelName in channelNames)
            {
                string queueName = $"{channelName}:{typeof(T).Name}";
                await _db.ListLeftPushAsync(queueName, datas.Select(e => new RedisValue(JsonConvert.SerializeObject(e))).ToArray());
                await _db.PublishAsync(channelName, queueName);
            }
        }
        #endregion
    }
}
