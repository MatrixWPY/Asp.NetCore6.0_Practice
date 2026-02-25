using RedLock.Services.Interface;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace RedLock.Services.Instance
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
        public async Task<T> AcquireLockAsync<T>(string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retry, Func<T> success, Func<T> fail)
        {
            T result = default(T);

            // Redis 分散式鎖參數說明
            // 1. resource : 鎖的資源名稱
            // 2. expiryTime : 鎖的過期時間
            // 3. waitTime : 未取得鎖的等待時間
            // 4. retryTime : 未取得鎖的重試間隔
            using (var redLock = await _redLockFactory.CreateLockAsync(resource, expiry, wait, retry))
            {
                if (redLock.IsAcquired)
                {
                    // 成功取得鎖的所有權，執行需要獨佔資源的核心工作
                    result = success();
                }
                else
                {
                    // 重試達到等待時間仍未取得鎖的所有權，執行失敗的後續處理
                    result = fail();
                }
            }
            // 離開 using 區塊時，RedLock.net 會自動執行 Lua Script 安全地釋放鎖

            return result;
        }
    }
}
