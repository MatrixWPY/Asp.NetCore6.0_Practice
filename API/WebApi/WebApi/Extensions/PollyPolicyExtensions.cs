using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace WebApi.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class PollyPolicyExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
        {
            return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync(
                        retryCount: 2,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (outcome, timespan, retryAttempt, context) =>
                        {
                            var statusCode = outcome?.Result?.StatusCode.ToString() ?? "N/A";
                            logger.LogWarning(
                                "Retry {RetryAttempt} after {Delay} seconds due to {Reason} (Status: {StatusCode})",
                                retryAttempt,
                                timespan.TotalSeconds,
                                outcome.Exception?.Message ?? outcome.Result?.ReasonPhrase ?? "Unknown error",
                                statusCode
                            );
                        }
                    );
        }
    }
}
