using RabbitMQ.Client;

namespace Consumer
{
    /// <summary>
    /// RabbitMQ Helper
    /// </summary>
    public class RabbitMQHelper
    {
        private static ConnectionFactory factory;
        private static object lockObj = new object();

        /// <summary>
        /// 獲取單個RabbitMQ連接
        /// </summary>
        /// <returns></returns>
        public static IConnection GetConnection()
        {
            if (factory == null)
            {
                lock (lockObj)
                {
                    if (factory == null)
                    {
                        factory = new ConnectionFactory
                        {
                            HostName = "localhost", //IP
                            Port = 5672,            //Port
                            UserName = "guest",     //帳號
                            Password = "guest",     //密碼
                            VirtualHost = "DevTest" //虛擬主機
                        };
                    }
                }
            }
            return factory.CreateConnection();
        }
    }
}
