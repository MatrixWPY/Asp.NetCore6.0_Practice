namespace WebApi.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRabbitMQService
    {
        /// <summary>
        /// 傳送資料至 RabbitMQ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="queueName"></param>
        /// <param name="value"></param>
        void SendDirect<T>(string exchange, string routingKey, string queueName, T value);

        /// <summary>
        /// 接收資料從 RabbitMQ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="cbFunc"></param>
        void ReceiveDirect<T>(string queueName, Func<T, bool> cbFunc);

        /// <summary>
        /// 接收資料從 RabbitMQ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="receiveCnt"></param>
        /// <param name="timeoutSec"></param>
        /// <param name="cbFunc"></param>
        void ReceiveDirect<T>(string queueName, int receiveCnt, int timeoutSec, Func<T, bool> cbFunc);
    }
}
