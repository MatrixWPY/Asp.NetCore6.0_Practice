using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using WebApi.Services.Interface;
using static WebApi.Models.WeatherForecast;

namespace WebApi.Services.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly ILogger<WeatherForecastService> _logger;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="httpClientFactory"></param>
        public WeatherForecastService(
            ILogger<WeatherForecastService> logger,
            IHttpClientFactory httpClientFactory
        )
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("WeatherForecastApiClient");
        }

        /// <summary>
        /// 查詢36小時天氣預報
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public async Task<F_C0032_001> GetThirtySixHourForecast(Dictionary<string, string> queryParams)
        {
            try
            {
                var relativeUrl = QueryHelpers.AddQueryString("api/v1/rest/datastore/F-C0032-001", queryParams);

                var res = await _httpClient.GetAsync(relativeUrl);
                res.EnsureSuccessStatusCode();
                var resJson = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<F_C0032_001>(resJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"查詢36小時天氣預報 - 失敗 : {ex.Message}");
                throw;
            }
        }
    }
}
