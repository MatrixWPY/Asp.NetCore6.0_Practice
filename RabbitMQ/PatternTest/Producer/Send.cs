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
    }
}
