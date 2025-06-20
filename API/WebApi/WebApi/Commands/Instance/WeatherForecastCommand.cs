using AutoMapper;
using WebApi.Commands.Interface;
using WebApi.DtoModels.Common;
using WebApi.DtoModels.WeatherForecast;
using WebApi.Services.Interface;

namespace WebApi.Commands.Instance
{
    /// <summary>
    /// 
    /// </summary>
    public class WeatherForecastCommand : BaseCommand, IWeatherForecastCommand
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IWeatherForecastService _weatherForecastService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="mapper"></param>
        /// <param name="weatherForecastService"></param>
        public WeatherForecastCommand(
            IConfiguration configuration,
            IMapper mapper,
            IWeatherForecastService weatherForecastService
        )
        {
            _configuration = configuration;
            _mapper = mapper;
            _weatherForecastService = weatherForecastService;
        }

        /// <summary>
        /// 查詢36小時天氣預報
        /// </summary>
        /// <param name="objRQ"></param>
        /// <returns></returns>
        public async Task<ApiResultRP<GetThirtySixHourForecastRP>> GetThirtySixHourForecast(GetThirtySixHourForecastRQ objRQ)
        {
            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(objRQ.City))
            {
                dicParams["locationName"] = objRQ.City;
            }
            dicParams["Authorization"] = _configuration["WeatherForecastApi:Authorization"];

            var res = await _weatherForecastService.GetThirtySixHourForecast(dicParams);

            if (res == null)
            {
                return FailRP<GetThirtySixHourForecastRP>(1, "No Data");
            }
            else
            {
                return SuccessRP(_mapper.Map<GetThirtySixHourForecastRP>(res.Records));
            }
        }
    }
}
