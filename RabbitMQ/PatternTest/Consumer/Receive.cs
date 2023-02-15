using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer
{
    public class Receive
    {
        public static void Simple()
        {
            //隊列名
            string queueName = "simple_order";
            //創建連接
            var connection = RabbitMQHelper.GetConnection();
            {
                //創建通道
                var channel = connection.CreateModel();
                {
                    //創建隊列
                    channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    int i = 0;
                    consumer.Received += (model, ea) =>
                    {
                        //消費者業務處理
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"{i},隊列{queueName}消費消息長度:{message.Length}");
                        i++;
                    };
                    channel.BasicConsume(queueName, true, consumer);
                }
            }
        }
    }
}
