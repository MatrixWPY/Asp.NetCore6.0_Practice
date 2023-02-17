using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Producer
{
    public class RPCServer
    {
        public static void RpcHandle()
        {
            //監聽的隊列名
            string queueNameReq = "rpc_queue_req";
            //創建連接
            var connection = RabbitMQHelper.GetConnection();
            {
                //創建通道
                var channel = connection.CreateModel();
                {
                    //創建隊列
                    channel.QueueDeclare(queue: queueNameReq, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //prefetchSize : 每條消息大小，一般設為0，表示不限制
                    //prefetchCount=1 : 告知RabbitMQ，不要同時給一個消費者推送多於 N 個消息，也確保了消費速度和性能
                    //global : 是否為全局設置
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    Console.WriteLine("【服務端】等待RPC請求...");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var props = ea.BasicProperties;

                        string response = null;

                        try
                        {
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine($"【服務端】接收到數據:{message}");
                            Thread.Sleep(5000);
                            response = $"{message}，處理完成";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("錯誤:" + e.Message);
                            response = "";
                        }
                        finally
                        {
                            //呼叫的隊列名
                            string queueNameRes = props.ReplyTo;

                            var replyProps = channel.CreateBasicProperties();
                            replyProps.CorrelationId = props.CorrelationId;//消息id

                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            //發送消息
                            channel.BasicPublish(exchange: "", routingKey: queueNameRes, basicProperties: replyProps, body: responseBytes);
                            Console.WriteLine($"【服務端】回覆處理結果:{response}");

                            //消息ack確認，告訴RabbitMQ這條消息處理完，可以從RabbitMQ刪除了
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                    };

                    channel.BasicConsume(queue: queueNameReq, autoAck: false, consumer: consumer);
                }
            }
        }
    }
}
