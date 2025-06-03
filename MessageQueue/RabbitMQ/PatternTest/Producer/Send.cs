using RabbitMQ.Client;
using System.Text;

namespace Producer
{
    public class Send
    {
        /// <summary>
        /// Simple 模式
        /// </summary>
        public static void Simple()
        {
            //隊列名
            string queueName = "simple_order";
            //創建連接
            using (var connection = RabbitMQHelper.GetConnection())
            {
                //創建通道
                using (var channel = connection.CreateModel())
                {
                    //創建隊列
                    channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    for (var i = 0; i < 10; i++)
                    {
                        string message = $"Hello RabbitMQ ~ {i + 1}";
                        var body = Encoding.UTF8.GetBytes(message);
                        //發送消息
                        channel.BasicPublish(exchange: "", routingKey: queueName, mandatory: false, basicProperties: null, body);
                        Console.WriteLine($"發送消息到隊列:{queueName}, 內容:{message}");
                    }
                }
            }
        }

        /// <summary>
        /// Worker 模式
        /// </summary>
        public static void Worker()
        {
            //隊列名
            string queueName = "worker_order";
            //創建連接
            using (var connection = RabbitMQHelper.GetConnection())
            {
                //創建通道
                using (var channel = connection.CreateModel())
                {
                    //創建隊列
                    channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;//消息持久化

                    for (var i = 0; i < 10; i++)
                    {
                        string message = $"Hello RabbitMQ ~ {i + 1}";
                        var body = Encoding.UTF8.GetBytes(message);
                        //發送消息
                        channel.BasicPublish(exchange: "", routingKey: queueName, mandatory: false, basicProperties: properties, body);
                        Console.WriteLine($"發送消息到隊列:{queueName}, 內容:{message}");
                    }
                }
            }
        }

        /// <summary>
        /// Publish/Subscribe 模式 (ExchangeType: Fanout)
        /// </summary>
        public static void Fanout()
        {
            //交換機名
            string exchangeName1 = "fanout_exchange1";
            string exchangeName2 = "fanout_exchange2";
            //隊列名
            string queueName1 = "fanout_queue1";
            string queueName2 = "fanout_queue2";
            string queueName3 = "fanout_queue3";
            //創建連接
            using (var connection = RabbitMQHelper.GetConnection())
            {
                //創建通道
                using (var channel = connection.CreateModel())
                {
                    //創建交換機:Fanout類型
                    channel.ExchangeDeclare(exchangeName1, ExchangeType.Fanout, durable: true);
                    channel.ExchangeDeclare(exchangeName2, ExchangeType.Fanout, durable: true);
                    //創建隊列
                    channel.QueueDeclare(queueName1, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queueName2, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queueName3, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //隊列綁定交換機，routingKey不用給值
                    channel.QueueBind(queue: queueName1, exchange: exchangeName1, routingKey: "");
                    channel.QueueBind(queue: queueName2, exchange: exchangeName1, routingKey: "");
                    channel.QueueBind(queue: queueName3, exchange: exchangeName2, routingKey: "");

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;//消息持久化

                    for (int i = 0; i < 10; i++)
                    {
                        string message = $"RabbitMQ Fanout_1 {i + 1} Message";
                        var body = Encoding.UTF8.GetBytes(message);
                        //發送消息
                        channel.BasicPublish(exchange: exchangeName1, routingKey: "", basicProperties: properties, body);
                        Console.WriteLine($"發送Fanout_1消息:{message}");
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        string message = $"RabbitMQ Fanout_2 {i + 1} Message";
                        var body = Encoding.UTF8.GetBytes(message);
                        //發送消息
                        channel.BasicPublish(exchange: exchangeName2, routingKey: "", basicProperties: properties, body);
                        Console.WriteLine($"發送Fanout_2消息:{message}");
                    }
                }
            }
        }

        /// <summary>
        /// Routing 模式 (ExchangeType: Direct)
        /// </summary>
        public static void Direct()
        {
            //交換機名
            string exchangeName = "direct_exchange";
            //隊列名
            string queueName1 = "direct_errorLog";
            string queueName2 = "direct_allLog";
            //創建連接
            using (var connection = RabbitMQHelper.GetConnection())
            {
                //創建通道
                using (var channel = connection.CreateModel())
                {
                    //創建交換機:Direct類型
                    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true);
                    //創建隊列
                    channel.QueueDeclare(queueName1, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queueName2, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //隊列綁定交換機
                    //direct_errorLog隊列綁定routingKey:error
                    channel.QueueBind(queue: queueName1, exchange: exchangeName, routingKey: "error");
                    //direct_allLog隊列綁定routingKey:info,error
                    channel.QueueBind(queue: queueName2, exchange: exchangeName, routingKey: "info");
                    channel.QueueBind(queue: queueName2, exchange: exchangeName, routingKey: "error");

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;//消息持久化

                    for (int i = 0; i < 10; i++)
                    {
                        string message = $"RabbitMQ Direct {i + 1} error Message";
                        var body = Encoding.UTF8.GetBytes(message);
                        //發送消息
                        channel.BasicPublish(exchange: exchangeName, routingKey: "error", basicProperties: properties, body);
                        Console.WriteLine($"發送Direct消息error:{message}");

                        string message2 = $"RabbitMQ Direct {i + 1} info Message";
                        var body2 = Encoding.UTF8.GetBytes(message2);
                        //發送消息
                        channel.BasicPublish(exchange: exchangeName, routingKey: "info", basicProperties: properties, body2);
                        Console.WriteLine($"發送Direct消息info:{message2}");
                    }
                }
            }
        }

        /// <summary>
        /// Topics 模式 (ExchangeType: Topic)
        /// </summary>
        public static void Topic()
        {
            //交換機名
            string exchangeName = "topic_exchange";
            //隊列名
            string queueName1 = "topic_queue1";
            string queueName2 = "topic_queue2";
            //路由名
            string routingKey1 = "*.orange.*";
            string routingKey2 = "*.*.rabbit";
            string routingKey3 = "lazy.#";
            //創建連接
            using (var connection = RabbitMQHelper.GetConnection())
            {
                //創建通道
                using (var channel = connection.CreateModel())
                {
                    //創建交換機:Topic類型
                    channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, durable: true);
                    //創建隊列
                    channel.QueueDeclare(queueName1, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queueName2, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //隊列綁定交換機，指定routingKey
                    channel.QueueBind(queue: queueName1, exchange: exchangeName, routingKey: routingKey1);
                    channel.QueueBind(queue: queueName2, exchange: exchangeName, routingKey: routingKey2);
                    channel.QueueBind(queue: queueName2, exchange: exchangeName, routingKey: routingKey3);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;//消息持久化

                    for (int i = 0; i < 10; i++)
                    {
                        string message = $"RabbitMQ Topic_1 {i + 1} Message";
                        var body = Encoding.UTF8.GetBytes(message);
                        //發送消息
                        channel.BasicPublish(exchange: exchangeName, routingKey: "aaa.orange.rabbit", basicProperties: properties, body);
                        Console.WriteLine($"發送Topic_1消息:{message}");

                        string message2 = $"RabbitMQ Topic_2 {i + 1} Message";
                        var body2 = Encoding.UTF8.GetBytes(message2);
                        //發送消息
                        channel.BasicPublish(exchange: exchangeName, routingKey: "lazy.aa.rabbit", basicProperties: properties, body2);
                        Console.WriteLine($"發送Topic_2消息:{message2}");
                    }
                }
            }
        }
    }
}
