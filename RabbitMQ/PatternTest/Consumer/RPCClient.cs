using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

namespace Consumer
{
    public class RPCClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;

        public RPCClient()
        {
            //監聽的隊列名
            string queueNameRes = "rpc_queue_res";
            //創建連接
            connection = RabbitMQHelper.GetConnection();
            //創建通道
            channel = connection.CreateModel();
            //創建隊列
            channel.QueueDeclare(queue: queueNameRes, durable: false, exclusive: true, autoDelete: true, arguments: null);

            var correlationId = Guid.NewGuid().ToString();
            props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;//消息id
            props.ReplyTo = queueNameRes;//回調的隊列名，Client關閉後會自動刪除

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);

                //監聽的消息Id和定義的消息Id相同，代表這條消息服務端處理完成
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };

            channel.BasicConsume(queue: queueNameRes, autoAck: true, consumer: consumer);
        }

        public string Call(string message)
        {
            //呼叫的隊列名
            string queueNameReq = "rpc_queue_req";
            var messageBytes = Encoding.UTF8.GetBytes(message);
            //發送消息
            channel.BasicPublish(exchange: "", routingKey: queueNameReq, basicProperties: props, body: messageBytes);
            //等待回覆
            return respQueue.Take();
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
