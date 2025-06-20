using AutoMapper;

namespace WebApi.Profiles
{
    /// <summary>
    /// 
    /// </summary>
    public class WeatherForecastProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public WeatherForecastProfile()
        {
            CreateMap<Models.WeatherForecast.Records, DtoModels.WeatherForecast.GetThirtySixHourForecastRP>();
            CreateMap<Models.WeatherForecast.Location, DtoModels.WeatherForecast.Location>();
            CreateMap<Models.WeatherForecast.WeatherElement, DtoModels.WeatherForecast.WeatherElement>();
            CreateMap<Models.WeatherForecast.TimeData, DtoModels.WeatherForecast.TimeData>();
            CreateMap<Models.WeatherForecast.Parameter, DtoModels.WeatherForecast.Parameter>();
        }
    }
}
