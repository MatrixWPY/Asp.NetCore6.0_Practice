namespace RedLock.Services.Interface
{
    public interface IRedlockService
    {
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
        Task<T> AcquireLockAsync<T>(string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retry, Func<T> success, Func<T> fail);
    }
}
