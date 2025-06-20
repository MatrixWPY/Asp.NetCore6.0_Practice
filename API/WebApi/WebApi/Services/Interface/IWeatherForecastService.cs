using AspectCore.DynamicProxy;
using WebApi.Interceptors;
using static WebApi.Models.WeatherForecast;

namespace WebApi.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceInterceptor(typeof(LogInterceptor))]
    public interface IWeatherForecastService
    {
        /// <summary>
        /// 查詢36小時天氣預報
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        Task<F_C0032_001> GetThirtySixHourForecast(Dictionary<string, string> queryParams);
    }
}
