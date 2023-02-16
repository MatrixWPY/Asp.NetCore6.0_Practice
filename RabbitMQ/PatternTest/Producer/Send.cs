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
                    //消息持久化
                    properties.Persistent = true;

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
        /// Publish/Subscribe 模式 (Fanout)
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
                    //創建交換機:fanout類型
                    channel.ExchangeDeclare(exchangeName1, ExchangeType.Fanout, durable: true);
                    channel.ExchangeDeclare(exchangeName2, ExchangeType.Fanout, durable: true);
                    //創建隊列
                    channel.QueueDeclare(queueName1, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queueName2, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queueName3, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //把創建的隊列綁定交換機，routingKey不用給值，給了也沒意義
                    channel.QueueBind(queue: queueName1, exchange: exchangeName1, routingKey: "");
                    channel.QueueBind(queue: queueName2, exchange: exchangeName1, routingKey: "");
                    channel.QueueBind(queue: queueName3, exchange: exchangeName2, routingKey: "");
                    var properties = channel.CreateBasicProperties();
                    //消息持久化
                    properties.Persistent = true;

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
    }
}
