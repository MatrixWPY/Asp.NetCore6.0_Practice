using RabbitMQ.Client;
using WebApi.Services.Interface;

namespace WebApi.Services.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitMQService : IRabbitMQService
    {
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private static object _connectionFactoryLock = new object();
        private readonly ILogger<RabbitMQService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public RabbitMQService(ILogger<RabbitMQService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connection = GetConnection(configuration);
        }

        private IConnection GetConnection(IConfiguration configuration)
        {
            if (_connectionFactory != null)
            {
                return _connectionFactory.CreateConnection();
            }
            lock (_connectionFactoryLock)
            {
                try
                {
                    _connectionFactory = new ConnectionFactory
                    {
                        HostName = configuration.GetValue<string>("RabbitMQ:HostName"),
                        Port = configuration.GetValue<int>("RabbitMQ:Port"),
                        UserName = configuration.GetValue<string>("RabbitMQ:UserName"),
                        Password = configuration.GetValue<string>("RabbitMQ:Password")
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError($"獲取RabbitMQ配置信息失敗：{ex.Message}");
                    return null;
                }
            }
            return _connectionFactory.CreateConnection();
        }
    }
}
