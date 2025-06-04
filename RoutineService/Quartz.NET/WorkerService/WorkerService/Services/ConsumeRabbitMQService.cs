using WorkerService.Helpers;

namespace WorkerService.Services
{
    public class ConsumeRabbitMQService
    {
        private readonly RabbitMQHelper _rabbitMQHelper;

        public ConsumeRabbitMQService(RabbitMQHelper rabbitMQHelper)
        {
            _rabbitMQHelper = rabbitMQHelper;
        }

        public void Consume<T>(Func<T, bool> func)
        {
            _rabbitMQHelper.ReceiveDirect<T>(typeof(T).Name, func);
        }
    }
}
