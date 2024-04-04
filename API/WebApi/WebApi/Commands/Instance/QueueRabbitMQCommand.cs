using WebApi.Commands.Interface;
using WebApi.Models.Response;
using WebApi.Services.Interface;

namespace WebApi.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class QueueRabbitMQCommand : BaseCommand, IQueueCommand
    {
        private readonly IRabbitMQService _rabbitMQService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rabbitMQService"></param>
        public QueueRabbitMQCommand(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        public ApiResultRP<IEnumerable<T>> Receive<T>(int cnt)
        {
            List<T> res = new List<T>();

            _rabbitMQService.ReceiveDirect<T>("ContactInfo", cnt, 3, (obj) =>
            {
                res.Add(obj);
                return true;
            });

            return SuccessRP(res.AsEnumerable());
        }

        public ApiResultRP<bool> Send<T>(T value)
        {
            _rabbitMQService.SendDirect("QueueCommand", "Send", "ContactInfo", value);
            return SuccessRP(true);
        }
    }
}
