using System.Diagnostics;
using WebApi.Commands.Interface;
using WebApi.DtoModels.Common;
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
            var res = _redisService.ReceiveStreamQueue<string,T>(nameof(QueueRedisStreamCommand), typeof(T).Name, $"{nameof(Receive)}_{Process.GetCurrentProcess().Id}", cnt);
            return SuccessRP(res.Select(e => e.Value));
        }

        public ApiResultRP<bool> Send<T>(T value)
        {
            var key = Guid.NewGuid().ToString("N");
            _redisService.SendStreamQueue(nameof(QueueRedisStreamCommand), typeof(T).Name, key, value);
            return SuccessRP(true);
        }
    }
}
