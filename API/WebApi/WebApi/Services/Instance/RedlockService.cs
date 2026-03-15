using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using WebApi.Services.Interface;

namespace WebApi.Services.Instance
{
    public class RedlockService : IRedlockService, IDisposable
    {
        private readonly RedLockFactory _redLockFactory;

        public RedlockService(RedisBaseService redisBaseService)
        {
            _redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { redisBaseService.Connection });
        }

        public async Task AcquireLockAsync(
            string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retry,
            Func<Task> success, Func<Task> fail = null)
        {
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
                    await success();
                }
                else
                {
                    // 重試達到等待時間仍未取得鎖的所有權，執行失敗的後續處理
                    if (fail != null)
                    {
                        await fail();
                    }
                }
            }
            // 離開 using 區塊時，RedLock.net 會自動執行 Lua Script 安全地釋放鎖
        }

        public async Task<T> AcquireLockAsync<T>(
            string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retry,
            Func<Task<T>> success, Func<Task<T>> fail = null)
        {
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
                    return await success();
                }
                else
                {
                    // 重試達到等待時間仍未取得鎖的所有權，執行失敗的後續處理
                    return fail != null ? await fail() : default;
                }
            }
            // 離開 using 區塊時，RedLock.net 會自動執行 Lua Script 安全地釋放鎖
        }

        // 隱藏的 Dispose，交給 DI 容器呼叫
        void IDisposable.Dispose()
        {
            _redLockFactory?.Dispose();
        }
    }
}
