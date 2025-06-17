using WebApi.DtoModels.Common;
using WebApi.DtoModels.WeatherForecast;

namespace WebApi.Commands.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWeatherForecastCommand
    {
        /// <summary>
        /// 查詢36小時天氣預報
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        Task<ApiResultRP<GetThirtySixHourForecastRP>> GetThirtySixHourForecast(GetThirtySixHourForecastRQ objRQ);
    }
}
