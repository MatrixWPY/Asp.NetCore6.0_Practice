using WorkerService.Helpers;

namespace WorkerService.Services
{
    public class SubscribeRedisListService
    {
        private readonly RedisHelper _redisHelper;

        public SubscribeRedisListService(RedisHelper redisHelper)
        {
            _redisHelper = redisHelper;
        }

        public async Task SubscribeAsync<T>(Func<T, Task<bool>> func)
        {
            await _redisHelper.SubscribeListQueueAsync("Channel_ListQueue", func);
        }
    }
}
