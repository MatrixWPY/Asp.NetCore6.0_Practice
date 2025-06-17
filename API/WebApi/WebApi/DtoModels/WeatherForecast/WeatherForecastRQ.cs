namespace WebApi.DtoModels.WeatherForecast
{
    /// <summary>
    /// 傳入參數 - 查詢36小時天氣預報
    /// </summary>
    public class GetThirtySixHourForecastRQ
    {
        public string City { get; set; }
    }
}
