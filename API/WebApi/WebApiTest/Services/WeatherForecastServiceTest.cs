using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NLog;
using NLog.Extensions.Logging;
using Polly;
using System.Net;
using WebApi.Services.Instance;
using WebApi.Services.Interface;
using static WebApi.Extensions.PollyPolicyExtensions;

namespace WebApiTest.Services
{
    public class WeatherForecastServiceTest
    {
        public WeatherForecastServiceTest()
        {
            LogManager.Setup().LoadConfigurationFromFile("nlog.config", optional: false);
        }

        [Theory]
        [MemberData(nameof(WeatherForecastServiceTestData.GetThirtySixHourForecast_RetryCases), MemberType = typeof(WeatherForecastServiceTestData))]
        public async Task GetThirtySixHourForecast_PollyRetry(Queue<HttpStatusCode> statusQueue, int expectedCallCount)
        {
            #region Arrange
            int callCount = 0;

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                                       "SendAsync",
                                       ItExpr.IsAny<HttpRequestMessage>(),
                                       ItExpr.IsAny<CancellationToken>()
                                   )
                                   .ReturnsAsync(() =>
                                   {
                                       callCount++;
                                       var statusCode = statusQueue.Dequeue();
                                       return new HttpResponseMessage
                                       {
                                           StatusCode = statusCode
                                       };
                                   });

            var services = new ServiceCollection();
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                logging.AddNLog();
            });
            services.AddHttpClient("WeatherForecastApiClient")
                    .ConfigureHttpClient(client =>
                    {
                        client.BaseAddress = new Uri("https://mock-api.com/");
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => handlerMock.Object)
                    .AddPolicyHandler((svc, req) =>
                    {
                        var logger = svc.GetRequiredService<ILoggerFactory>()
                                        .CreateLogger("PollyRetryPolicy");

                        return Policy.WrapAsync(
                            GetRetryPolicy(logger),
                            Policy.TimeoutAsync<HttpResponseMessage>(30)
                        );
                    });
            services.AddTransient<IWeatherForecastService, WeatherForecastService>();
            #endregion

            #region Act
            var provider = services.BuildServiceProvider();
            var processor = provider.GetRequiredService<IWeatherForecastService>();
            var result = await processor.GetThirtySixHourForecast(new Dictionary<string, string>());
            #endregion

            #region Assert
            Assert.Equal(expectedCallCount, callCount);
            #endregion
        }
    }

    public static class WeatherForecastServiceTestData
    {
        public static IEnumerable<object[]> GetThirtySixHourForecast_RetryCases => new List<object[]>
        {
            new object[]
            {
                new Queue<HttpStatusCode>(new[]
                {
                    HttpStatusCode.InternalServerError,
                    HttpStatusCode.TooManyRequests,
                    HttpStatusCode.OK
                }),
                3
            },
            new object[]
            {
                new Queue<HttpStatusCode>(new[]
                {
                    HttpStatusCode.ServiceUnavailable,
                    HttpStatusCode.RequestTimeout,
                    HttpStatusCode.OK
                }),
                3
            },
            new object[]
            {
                new Queue<HttpStatusCode>(new[]
                {
                    HttpStatusCode.RequestTimeout,
                    HttpStatusCode.TooManyRequests,
                    HttpStatusCode.OK
                }),
                3
            }
        };
    }
}
