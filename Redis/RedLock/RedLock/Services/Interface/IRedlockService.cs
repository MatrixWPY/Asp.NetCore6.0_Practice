using RedLockNet.SERedis;

namespace RedLock.Services.Interface
{
    public interface IRedlockService
    {
        RedLockFactory RedisLockFactory { get; }
    }
}
