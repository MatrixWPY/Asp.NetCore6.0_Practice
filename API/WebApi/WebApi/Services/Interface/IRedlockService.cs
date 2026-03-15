namespace WebApi.Services.Interface
{
    public interface IRedlockService
    {
        void AcquireLock(
            string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retry,
            Action success, Action fail = null);

        Task AcquireLockAsync(
            string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retry,
            Func<Task> success, Func<Task> fail = null);

        T AcquireLock<T>(
            string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retry,
            Func<T> success, Func<T> fail = null);

        Task<T> AcquireLockAsync<T>(
            string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retry,
            Func<Task<T>> success, Func<Task<T>> fail = null);
    }
}
