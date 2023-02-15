using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;

namespace Consumer
{
    public class Receive
    {
        /// <summary>
        /// Simple 模式
        /// </summary>
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
                    consumer.Received += (model, ea) =>
                    {
                        //消費者業務處理
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"接收消息從隊列:{queueName}, 內容:{message}");
                    };
                    channel.BasicConsume(queueName, true, consumer);
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
            var connection = RabbitMQHelper.GetConnection();
            {
                //創建通道
                var channel = connection.CreateModel();
                {
                    //創建隊列
                    channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //prefetchSize : 每條消息大小，一般設為0，表示不限制
                    //prefetchCount=1 : 告知RabbitMQ，不要同時給一個消費者推送多於 N 個消息，也確保了消費速度和性能
                    //global : 是否為全局設置
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    int pId = Process.GetCurrentProcess().Id;
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        //消費者業務處理
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"ProcessId:{pId}, 接收消息從隊列:{queueName}, 內容:{message}");

                        //消息ack確認，告訴RabbitMQ這條消息處理完，可以從RabbitMQ刪除了
                        channel.BasicAck(ea.DeliveryTag, false);

                        Thread.Sleep(1000);
                    };
                    channel.BasicConsume(queueName, autoAck: false, consumer);
                }
            }
        }
    }
}
