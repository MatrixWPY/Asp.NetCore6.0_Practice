using WebApi.Commands.Interface;
using WebApi.Models.Response;
using WebApi.Services.Interface;

namespace WebApi.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class QueueRedisListCommand : BaseCommand, IQueueCommand
    {
        private readonly IRedisService _redisService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisService"></param>
        public QueueRedisListCommand(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public ApiResultRP<IEnumerable<T>> Receive<T>(int cnt)
        {
            var res = _redisService.ReceiveListQueue<T>($"{nameof(QueueRedisListCommand)}_{typeof(T).Name}", cnt);
            return SuccessRP(res);
        }

        public ApiResultRP<bool> Send<T>(T value)
        {
            _redisService.SendListQueue($"{nameof(QueueRedisListCommand)}_{typeof(T).Name}", value);
            return SuccessRP(true);
        }
    }
}
