using StackExchange.Redis;

namespace WebApi.Services
{
    public class RedisBaseService : IDisposable
    {
        private readonly ILogger<RedisBaseService> _logger;
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        public RedisBaseService(ILogger<RedisBaseService> logger, IConfiguration configuration)
        {
            _logger = logger;
            ConfigurationOptions options = ReadRedisSetting(configuration);
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
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

        public ConnectionMultiplexer Connection => _lazyConnection.Value;

        // 隱藏的 Dispose，交給 DI 容器呼叫
        void IDisposable.Dispose()
        {
            if (_lazyConnection.IsValueCreated)
            {
                _lazyConnection.Value.Dispose();
            }
        }
    }
}
