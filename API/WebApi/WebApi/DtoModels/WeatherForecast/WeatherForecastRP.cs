namespace WebApi.DtoModels.WeatherForecast
{
    /// <summary>
    /// 傳出參數 - 查詢36小時天氣預報
    /// </summary>
    public class GetThirtySixHourForecastRP
    {
        public string DatasetDescription { get; set; }
        public List<Location> Location { get; set; }
    }

    public class Location
    {
        public string LocationName { get; set; }
        public List<WeatherElement> WeatherElement { get; set; }
    }

    public class WeatherElement
    {
        public string ElementName { get; set; }
        public List<TimeData> Time { get; set; }
    }

    public class TimeData
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Parameter Parameter { get; set; }
    }

    public class Parameter
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterUnit { get; set; }
    }
}
