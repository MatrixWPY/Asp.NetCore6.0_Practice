using WebApi.Commands.Interface;
using WebApi.Services.Interface;

namespace WebApi.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class SubscribeRabbitMQCommand : ISubscribeCommand
    {
        private readonly IRabbitMQService _rabbitMQService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rabbitMQService"></param>
        public SubscribeRabbitMQCommand(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        public void Subscribe<T>(Func<T, bool> func)
        {
            _rabbitMQService.ReceiveDirect<T>(typeof(T).Name, func);
        }
    }
}
