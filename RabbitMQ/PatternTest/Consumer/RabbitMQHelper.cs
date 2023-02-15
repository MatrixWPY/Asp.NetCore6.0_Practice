using RabbitMQ.Client;

namespace Consumer
{
    /// <summary>
    /// RabbitMQ帮助类
    /// </summary>
    public class RabbitMQHelper
    {
        private static ConnectionFactory factory;
        private static object lockObj = new object();

        /// <summary>
        /// 获取单个RabbitMQ连接
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
                            HostName = "localhost", //ip
                            Port = 5672,            //端口
                            UserName = "guest",     //账号
                            Password = "guest",     //密码
                            VirtualHost = "DevTest" //虚拟主机
                        };
                    }
                }
            }
            return factory.CreateConnection();
        }
    }
}
