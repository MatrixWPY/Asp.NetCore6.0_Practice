using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace WorkerService.Helpers
{
    public class RabbitMQHelper
    {
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private static object _connectionFactoryLock = new object();
        private readonly ILogger<RabbitMQHelper> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public RabbitMQHelper(ILogger<RabbitMQHelper> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connection = GetConnection(configuration);
        }

        private IConnection GetConnection(IConfiguration configuration)
        {
            if (_connection != null)
            {
                return _connection;
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
                        Password = configuration.GetValue<string>("RabbitMQ:Password"),
                        AutomaticRecoveryEnabled = true,
                        NetworkRecoveryInterval = TimeSpan.FromMilliseconds(500)
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError($"獲取RabbitMQ配置信息失敗：{ex.Message}");
                    throw;
                }
            }
            return _connectionFactory.CreateConnection();
        }

        public void SendDirect<T>(string exchange, string routingKey, string queueName, T value)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true);
                channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(queue: queueName, exchange: exchange, routingKey);

                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Persistent = true;//消息持久化

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));

                channel.BasicPublish(exchange, routingKey, basicProperties, body);
            }
        }

        public void ReceiveDirect<T>(string queueName, Func<T, bool> cbFunc)
        {
            var channel = _connection.CreateModel();
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            //prefetchSize : 每條消息大小，一般設為0，表示不限制
            //prefetchCount=N : 告知RabbitMQ，不要同時給一個消費者推送多於 N 個消息，也確保了消費速度和性能
            //global : 是否為全局設置
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                var isProcess = cbFunc(JsonConvert.DeserializeObject<T>(message));
                if (isProcess)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            channel.BasicConsume(queueName, autoAck: false, consumer);
        }
    }
}
