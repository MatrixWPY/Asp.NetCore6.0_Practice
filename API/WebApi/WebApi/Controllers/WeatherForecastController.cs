using Microsoft.AspNetCore.Mvc;
using WebApi.Commands.Interface;
using WebApi.DtoModels.Common;
using WebApi.DtoModels.WeatherForecast;
using WebApi.Filters;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ValidRequest]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastCommand _weatherForecastCommand; 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weatherForecastCommand"></param>
        public WeatherForecastController(IWeatherForecastCommand weatherForecastCommand)
        {
            _weatherForecastCommand = weatherForecastCommand;
        }

        /// <summary>
        /// 查詢36小時天氣預報
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        [HttpGet, Route("GetThirtySixHourForecast")]
        public async Task<ApiResultRP<GetThirtySixHourForecastRP>> GetThirtySixHourForecast([FromQuery] GetThirtySixHourForecastRQ objRQ)
        {
            return await _weatherForecastCommand.GetThirtySixHourForecast(objRQ);
        }
    }
}
