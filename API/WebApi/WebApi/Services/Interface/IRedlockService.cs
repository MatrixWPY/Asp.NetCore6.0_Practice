namespace WebApi.Services.Interface
{
    public interface IRedlockService
    {
        Task AcquireLockAsync(
            string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retry,
            Func<Task> success, Func<Task> fail = null);

        Task<T> AcquireLockAsync<T>(
            string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retry,
            Func<Task<T>> success, Func<Task<T>> fail = null);
    }
}
