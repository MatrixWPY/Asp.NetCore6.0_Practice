using System.Threading.RateLimiting;

namespace WebApi.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public class RateLimitMiddleware
    {
        private readonly ILogger<RateLimitMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly RateLimiter _concurrencyLimiter;
        private readonly RateLimiter _tokenBucketLimiter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="next"></param>
        /// <param name="concurrencyLimiter"></param>
        /// <param name="tokenBucketLimiter"></param>
        public RateLimitMiddleware(
            ILogger<RateLimitMiddleware> logger,
            RequestDelegate next,
            RateLimiter concurrencyLimiter,
            RateLimiter tokenBucketLimiter
        )
        {
            _logger = logger;
            _next = next;
            _concurrencyLimiter = concurrencyLimiter;
            _tokenBucketLimiter = tokenBucketLimiter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            using var concurrencyLease = await _concurrencyLimiter.AcquireAsync(1);
            if (!concurrencyLease.IsAcquired)
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Too many concurrent requests.");
                _logger.LogWarning("Concurrency limit reached for {Path}", context.Request.Path);
                return;
            }

            using var tokenLease = await _tokenBucketLimiter.AcquireAsync(1);
            if (!tokenLease.IsAcquired)
            {
                concurrencyLease.Dispose();
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Rate limit exceeded (token bucket).");
                _logger.LogWarning("Token bucket limit reached for {Path}", context.Request.Path);
                return;
            }

            await _next(context);
        }
    }
}
