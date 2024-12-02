using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using WebMVC.Services.Interface;

namespace WebMVC.Services.Instance
{
    public class RedlockService : IRedlockService
    {
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly RedLockFactory _redLockFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisBase"></param>
        public RedlockService(IRedisBase redisBase)
        {
            _redisConnection = redisBase.ConnectionRedis();
            _redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { _redisConnection });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="expiry"></param>
        /// <param name="wait"></param>
        /// <param name="retry"></param>
        /// <param name="success"></param>
        /// <param name="fail"></param>
        /// <returns></returns>
        public async Task<T> AcquireLockAsync<T>(string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retry, Func<Task<T>> success, Func<Task<T>> fail)
        {
            T result = default(T);

            // blocks 直到取得 lock 資源或是達到放棄重試時間
            using (var redLock = await _redLockFactory.CreateLockAsync(resource, expiry, wait, retry))
            {
                // 確定取得 lock 所有權
                if (redLock.IsAcquired)
                {
                    // 執行需要獨佔資源的核心工作
                    result = await success();
                }
                // 未取得 lock 所有權
                else
                {
                    // 執行未取得 lock 的後續工作
                    result = await fail();
                }
            }
            // 脫離 using 範圍自動就會解除 lock

            return result;
        }
    }
}
