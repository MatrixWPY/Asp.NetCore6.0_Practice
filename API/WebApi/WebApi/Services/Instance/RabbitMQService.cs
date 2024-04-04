using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
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
        private ManualResetEventSlim _waitReceived;
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
                    return null;
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
            _waitReceived = new ManualResetEventSlim();

            using (var channel = _connection.CreateModel())
            {
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
                _waitReceived.Wait();
            }
        }

        public void ReceiveDirect<T>(string queueName, int receiveCnt, int timeoutSec, Func<T, bool> cbFunc)
        {
            _waitReceived = new ManualResetEventSlim();
            bool continueReceiving = true;
            int successCnt = 0;
            DateTime lastReceiveTime = DateTime.Now;

            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                //prefetchSize : 每條消息大小，一般設為0，表示不限制
                //prefetchCount=N : 告知RabbitMQ，不要同時給一個消費者推送多於 N 個消息，也確保了消費速度和性能
                //global : 是否為全局設置
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    if (continueReceiving == false)
                    {
                        return;
                    }

                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                    var isProcess = cbFunc(JsonConvert.DeserializeObject<T>(message));
                    if (isProcess)
                    {
                        channel.BasicAck(ea.DeliveryTag, false);

                        // 每次接收到訊息時，重置最後接收時間
                        lastReceiveTime = DateTime.Now;

                        successCnt++;
                        // 達到欲接收數量時，停止接收訊息
                        if (successCnt == receiveCnt)
                        {
                            continueReceiving = false;
                            _waitReceived.Set();
                        }
                    }
                };

                // 在一定時間內沒有收到訊息時停止接收
                Task.Run(() =>
                {
                    while (continueReceiving)
                    {
                        if (DateTime.Now - lastReceiveTime > TimeSpan.FromSeconds(timeoutSec))
                        {
                            continueReceiving = false;
                            _waitReceived.Set();
                        }
                        else
                        {
                            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                        }
                    }
                });

                channel.BasicConsume(queueName, autoAck: false, consumer);
                _waitReceived.Wait();
            }
        }
    }
}
