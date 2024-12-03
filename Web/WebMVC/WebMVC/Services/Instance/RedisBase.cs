using StackExchange.Redis;
using WebMVC.Services.Interface;

namespace WebMVC.Services.Instance
{
    public class RedisBase: IRedisBase
    {
        private static object _redisConnectionLock = new object();
        private volatile ConnectionMultiplexer _redisConnection;
        private readonly ConfigurationOptions _configOptions;
        private readonly ILogger<RedisBase> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public RedisBase(ILogger<RedisBase> logger, IConfiguration configuration)
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
                    DefaultDatabase = configuration.GetValue<int>("Redis:Db"),
                };
                return options;
            }
            catch (Exception ex)
            {
                _logger.LogError($"獲取Redis配置信息失敗：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConnectionMultiplexer ConnectionRedis()
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
    }
}
