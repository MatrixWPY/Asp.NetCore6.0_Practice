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
                    channel.BasicConsume(queueName, autoAck: true, consumer);
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

        /// <summary>
        /// Publish/Subscribe 模式 (ExchangeType: Fanout)
        /// </summary>
        public static void Fanout()
        {
            //隊列名
            string queueName1 = "fanout_queue1";
            string queueName2 = "fanout_queue2";
            string queueName3 = "fanout_queue3";
            //創建連接
            var connection = RabbitMQHelper.GetConnection();
            {
                //創建通道
                var channel1 = connection.CreateModel();
                {
                    //創建隊列
                    channel1.QueueDeclare(queueName1, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //prefetchSize : 每條消息大小，一般設為0，表示不限制
                    //prefetchCount=1 : 告知RabbitMQ，不要同時給一個消費者推送多於 N 個消息，也確保了消費速度和性能
                    //global : 是否為全局設置
                    channel1.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    int pId = Process.GetCurrentProcess().Id;
                    var consumer = new EventingBasicConsumer(channel1);
                    consumer.Received += (model, ea) =>
                    {
                        //消費者業務處理
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"ProcessId:{pId}, 接收消息從隊列:{queueName1}, 內容:{message}");

                        //消息ack確認，告訴RabbitMQ這條消息處理完，可以從RabbitMQ刪除了
                        channel1.BasicAck(ea.DeliveryTag, false);

                        Thread.Sleep(1000);
                    };
                    channel1.BasicConsume(queueName1, autoAck: false, consumer);
                }
                //創建通道
                var channel2 = connection.CreateModel();
                {
                    //創建隊列
                    channel2.QueueDeclare(queueName2, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //prefetchSize : 每條消息大小，一般設為0，表示不限制
                    //prefetchCount=1 : 告知RabbitMQ，不要同時給一個消費者推送多於 N 個消息，也確保了消費速度和性能
                    //global : 是否為全局設置
                    channel2.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    int pId = Process.GetCurrentProcess().Id;
                    var consumer = new EventingBasicConsumer(channel2);
                    consumer.Received += (model, ea) =>
                    {
                        //消費者業務處理
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"ProcessId:{pId}, 接收消息從隊列:{queueName2}, 內容:{message}");

                        //消息ack確認，告訴RabbitMQ這條消息處理完，可以從RabbitMQ刪除了
                        channel2.BasicAck(ea.DeliveryTag, false);

                        Thread.Sleep(1000);
                    };
                    channel2.BasicConsume(queueName2, autoAck: false, consumer);
                }
                //創建通道
                var channel3 = connection.CreateModel();
                {
                    //創建隊列
                    channel3.QueueDeclare(queueName3, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //prefetchSize : 每條消息大小，一般設為0，表示不限制
                    //prefetchCount=1 : 告知RabbitMQ，不要同時給一個消費者推送多於 N 個消息，也確保了消費速度和性能
                    //global : 是否為全局設置
                    channel3.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    int pId = Process.GetCurrentProcess().Id;
                    var consumer = new EventingBasicConsumer(channel3);
                    consumer.Received += (model, ea) =>
                    {
                        //消費者業務處理
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"ProcessId:{pId}, 接收消息從隊列:{queueName3}, 內容:{message}");

                        //消息ack確認，告訴RabbitMQ這條消息處理完，可以從RabbitMQ刪除了
                        channel3.BasicAck(ea.DeliveryTag, false);

                        Thread.Sleep(1000);
                    };
                    channel3.BasicConsume(queueName3, autoAck: false, consumer);
                }
            }
        }

        /// <summary>
        /// Routing 模式 (ExchangeType: Direct)
        /// </summary>
        public static void Direct()
        {
            //隊列名
            string queueName1 = "direct_errorLog";
            string queueName2 = "direct_allLog";
            //創建連接
            var connection = RabbitMQHelper.GetConnection();
            {
                //創建通道
                var channel1 = connection.CreateModel();
                {
                    //創建隊列
                    channel1.QueueDeclare(queueName1, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //prefetchSize : 每條消息大小，一般設為0，表示不限制
                    //prefetchCount=1 : 告知RabbitMQ，不要同時給一個消費者推送多於 N 個消息，也確保了消費速度和性能
                    //global : 是否為全局設置
                    channel1.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    int pId = Process.GetCurrentProcess().Id;
                    var consumer = new EventingBasicConsumer(channel1);
                    consumer.Received += (model, ea) =>
                    {
                        //消費者業務處理
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"ProcessId:{pId}, 接收消息從隊列:{queueName1}, 內容:{message}");

                        //消息ack確認，告訴RabbitMQ這條消息處理完，可以從RabbitMQ刪除了
                        channel1.BasicAck(ea.DeliveryTag, false);

                        Thread.Sleep(1000);
                    };
                    channel1.BasicConsume(queueName1, autoAck: false, consumer);
                }
                //創建通道
                var channel2 = connection.CreateModel();
                {
                    //創建隊列
                    channel2.QueueDeclare(queueName2, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //prefetchSize : 每條消息大小，一般設為0，表示不限制
                    //prefetchCount=1 : 告知RabbitMQ，不要同時給一個消費者推送多於 N 個消息，也確保了消費速度和性能
                    //global : 是否為全局設置
                    channel2.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    int pId = Process.GetCurrentProcess().Id;
                    var consumer = new EventingBasicConsumer(channel2);
                    consumer.Received += (model, ea) =>
                    {
                        //消費者業務處理
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"ProcessId:{pId}, 接收消息從隊列:{queueName2}, 內容:{message}");

                        //消息ack確認，告訴RabbitMQ這條消息處理完，可以從RabbitMQ刪除了
                        channel2.BasicAck(ea.DeliveryTag, false);

                        Thread.Sleep(1000);
                    };
                    channel2.BasicConsume(queueName2, autoAck: false, consumer);
                }
            }
        }

        /// <summary>
        /// Topics 模式 (ExchangeType: Topic)
        /// </summary>
        public static void Topic()
        {
            //隊列名
            string queueName1 = "topic_queue1";
            string queueName2 = "topic_queue2";
            //創建連接
            var connection = RabbitMQHelper.GetConnection();
            {
                //創建通道
                var channel1 = connection.CreateModel();
                {
                    //創建隊列
                    channel1.QueueDeclare(queueName1, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //prefetchSize : 每條消息大小，一般設為0，表示不限制
                    //prefetchCount=1 : 告知RabbitMQ，不要同時給一個消費者推送多於 N 個消息，也確保了消費速度和性能
                    //global : 是否為全局設置
                    channel1.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    int pId = Process.GetCurrentProcess().Id;
                    var consumer = new EventingBasicConsumer(channel1);
                    consumer.Received += (model, ea) =>
                    {
                        //消費者業務處理
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"ProcessId:{pId}, 接收消息從隊列:{queueName1}, 內容:{message}");

                        //消息ack確認，告訴RabbitMQ這條消息處理完，可以從RabbitMQ刪除了
                        channel1.BasicAck(ea.DeliveryTag, false);

                        Thread.Sleep(1000);
                    };
                    channel1.BasicConsume(queueName1, autoAck: false, consumer);
                }
                //創建通道
                var channel2 = connection.CreateModel();
                {
                    //創建隊列
                    channel2.QueueDeclare(queueName2, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //prefetchSize : 每條消息大小，一般設為0，表示不限制
                    //prefetchCount=1 : 告知RabbitMQ，不要同時給一個消費者推送多於 N 個消息，也確保了消費速度和性能
                    //global : 是否為全局設置
                    channel2.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    int pId = Process.GetCurrentProcess().Id;
                    var consumer = new EventingBasicConsumer(channel2);
                    consumer.Received += (model, ea) =>
                    {
                        //消費者業務處理
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"ProcessId:{pId}, 接收消息從隊列:{queueName2}, 內容:{message}");

                        //消息ack確認，告訴RabbitMQ這條消息處理完，可以從RabbitMQ刪除了
                        channel2.BasicAck(ea.DeliveryTag, false);

                        Thread.Sleep(1000);
                    };
                    channel2.BasicConsume(queueName2, autoAck: false, consumer);
                }
            }
        }
    }
}
