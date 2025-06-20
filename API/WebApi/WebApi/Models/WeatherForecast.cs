namespace WebApi.Models
{
    /// <summary>
    /// WeatherForecast - API物件
    /// </summary>
    public class WeatherForecast
    {
        public class F_C0032_001
        {
            public string Success { get; set; }
            public Result Result { get; set; }
            public Records Records { get; set; }
        }

        public class Result
        {
            public string ResourceId { get; set; }
            public List<Field> Fields { get; set; }
        }

        public class Field
        {
            public string Id { get; set; }
            public string Type { get; set; }
        }

        public class Records
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
}
