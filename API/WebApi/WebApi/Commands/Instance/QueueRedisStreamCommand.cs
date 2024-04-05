using WebApi.Commands.Interface;
using WebApi.Models.Response;
using WebApi.Services.Interface;

namespace WebApi.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class QueueRedisStreamCommand : BaseCommand, IQueueCommand
    {
        private readonly IRedisService _redisService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisService"></param>
        public QueueRedisStreamCommand(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public ApiResultRP<IEnumerable<T>> Receive<T>(int cnt)
        {
            var res = _redisService.ReceiveStreamQueue<string,T>("QueueRedisStreamCommand", "ContactInfo", "Receive", cnt);
            return SuccessRP(res.Select(e => e.Value));
        }

        public ApiResultRP<bool> Send<T>(T value)
        {
            var key = Guid.NewGuid().ToString("N");
            _redisService.SendStreamQueue("QueueRedisStreamCommand", "ContactInfo", key, value);
            return SuccessRP(true);
        }
    }
}
