using Newtonsoft.Json;
using StackExchange.Redis;

namespace WorkerService.Helpers
{
    public class RedisHelper
    {
        public volatile ConnectionMultiplexer _redisConnection;
        private static object _redisConnectionLock = new object();
        private readonly ConfigurationOptions _configOptions;
        private readonly ILogger<RedisHelper> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public RedisHelper(ILogger<RedisHelper> logger, IConfiguration configuration)
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
                    ClientName = configuration.GetValue<string>("Redis:Name"),
                    EndPoints =
                    {
                        {
                            configuration.GetValue<string>("Redis:Ip"),
                            configuration.GetValue<int>("Redis:Port")
                        }
                    },
                    Password = configuration.GetValue<string>("Redis:Password"),
                    DefaultDatabase = configuration.GetValue<int>("Redis:Db")
                };
                return options;
            }
            catch (Exception ex)
            {
                _logger.LogError($"獲取Redis配置信息失敗：{ex.Message}");
                throw;
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
                    throw;
                }
            }
            return _redisConnection;
        }

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

            var sub = _redisConnection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            sub.Subscribe(channel, (chl, msg) =>
            {
                string queueName = msg;
                if (queueName.EndsWith($":{typeof(T).Name}") == false)
                {
                    return;
                }

                var db = _redisConnection.GetDatabase();
                while (db.ListLength(queueName) > 0)
                {
                    var data = db.ListRightPop(queueName);
                    if (data.IsNullOrEmpty)
                    {
                        break;
                    }

                    action(JsonConvert.DeserializeObject<T>(data));
                }
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

            var sub = _redisConnection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            await sub.SubscribeAsync(channel, async (chl, msg) =>
            {
                string queueName = msg;
                if (queueName.EndsWith($":{typeof(T).Name}") == false)
                {
                    return;
                }

                var db = _redisConnection.GetDatabase();
                while (await db.ListLengthAsync(queueName) > 0)
                {
                    var data = await db.ListRightPopAsync(queueName);
                    if (data.IsNullOrEmpty)
                    {
                        break;
                    }

                    await func(JsonConvert.DeserializeObject<T>(data));
                }
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

            var sub = _redisConnection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            sub.Subscribe(channel, (chl, msg) =>
            {
                string queueName = msg;
                if (queueName.EndsWith($":{typeof(T).Name}") == false)
                {
                    return;
                }

                var db = _redisConnection.GetDatabase();
                var datas = db.ListRightPop(queueName, db.ListLength(queueName));
                if (datas == null)
                {
                    return;
                }

                action(datas.Select(e => JsonConvert.DeserializeObject<T>(e)));
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

            var sub = _redisConnection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            await sub.SubscribeAsync(channel, async (chl, msg) =>
            {
                string queueName = msg;
                if (queueName.EndsWith($":{typeof(T).Name}") == false)
                {
                    return;
                }

                var db = _redisConnection.GetDatabase();
                var datas = await db.ListRightPopAsync(queueName, await db.ListLengthAsync(queueName));
                if (datas == null)
                {
                    return;
                }

                await func(datas.Select(e => JsonConvert.DeserializeObject<T>(e)));
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

            var sub = _redisConnection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            sub.Subscribe(channel, (chl, msg) =>
            {
                string queueName = msg;
                if (queueName.EndsWith($":{typeof(T).Name}") == false)
                {
                    return;
                }

                var db = _redisConnection.GetDatabase();
                while (db.ListLength(queueName) > 0)
                {
                    var data = db.ListRightPop(queueName);
                    if (data.IsNullOrEmpty)
                    {
                        break;
                    }

                    var res = func(JsonConvert.DeserializeObject<T>(data));
                    if (res == false)
                    {
                        db.ListRightPush(queueName, data);
                    }
                }
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

            var sub = _redisConnection.GetSubscriber();
            var patternMode = channelName.StartsWith("*") || channelName.EndsWith("*")
                            ? RedisChannel.PatternMode.Pattern : RedisChannel.PatternMode.Literal;
            var channel = new RedisChannel(channelName, patternMode);

            await sub.SubscribeAsync(channel, async (chl, msg) =>
            {
                string queueName = msg;
                if (queueName.EndsWith($":{typeof(T).Name}") == false)
                {
                    return;
                }

                var db = _redisConnection.GetDatabase();

                while (await db.ListLengthAsync(queueName) > 0)
                {
                    var data = await db.ListRightPopAsync(queueName);
                    if (data.IsNullOrEmpty)
                    {
                        break;
                    }

                    var res = await funcAsync(JsonConvert.DeserializeObject<T>(data));
                    if (res == false)
                    {
                        await db.ListRightPushAsync(queueName, data);
                    }
                }
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

            var db = _redisConnection.GetDatabase();
            string queueName = $"{channelName}:{typeof(T).Name}";
            db.ListLeftPush(queueName, JsonConvert.SerializeObject(data));
            db.Publish(channelName, queueName);
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

            var db = _redisConnection.GetDatabase();
            string queueName = $"{channelName}:{typeof(T).Name}";
            await db.ListLeftPushAsync(queueName, JsonConvert.SerializeObject(data));
            await db.PublishAsync(channelName, queueName);
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

            var db = _redisConnection.GetDatabase();
            string queueName = $"{channelName}:{typeof(T).Name}";
            db.ListLeftPush(queueName, datas.Select(e => new RedisValue(JsonConvert.SerializeObject(e))).ToArray());
            db.Publish(channelName, queueName);
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

            var db = _redisConnection.GetDatabase();
            string queueName = $"{channelName}:{typeof(T).Name}";
            await db.ListLeftPushAsync(queueName, datas.Select(e => new RedisValue(JsonConvert.SerializeObject(e))).ToArray());
            await db.PublishAsync(channelName, queueName);
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

            var db = _redisConnection.GetDatabase();
            foreach (var channelName in channelNames)
            {
                string queueName = $"{channelName}:{typeof(T).Name}";
                db.ListLeftPush(queueName, JsonConvert.SerializeObject(data));
                db.Publish(channelName, queueName);
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

            var db = _redisConnection.GetDatabase();
            foreach (var channelName in channelNames)
            {
                string queueName = $"{channelName}:{typeof(T).Name}";
                await db.ListLeftPushAsync(queueName, JsonConvert.SerializeObject(data));
                await db.PublishAsync(channelName, queueName);
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

            var db = _redisConnection.GetDatabase();
            foreach (var channelName in channelNames)
            {
                string queueName = $"{channelName}:{typeof(T).Name}";
                db.ListLeftPush(queueName, datas.Select(e => new RedisValue(JsonConvert.SerializeObject(e))).ToArray());
                db.Publish(channelName, queueName);
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

            var db = _redisConnection.GetDatabase();
            foreach (var channelName in channelNames)
            {
                string queueName = $"{channelName}:{typeof(T).Name}";
                await db.ListLeftPushAsync(queueName, datas.Select(e => new RedisValue(JsonConvert.SerializeObject(e))).ToArray());
                await db.PublishAsync(channelName, queueName);
            }
        }
        #endregion
    }
}
