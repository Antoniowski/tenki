namespace Tenki
{
    public class WeatherCurrentUnits
    {
        public string? time { get; set; }
        public string? interval { get; set; }
        public string? temperature_2m { get; set; }
        public string? relative_humidity_2m { get; set; }
        public string? is_day { get; set; }
        public string? precipitation { get; set; }
        public string? weather_code { get; set; }
    }

    public class WeatherCurrent
    {
        public DateTime time { get; set; }
        public int interval { get; set; }
        public double temperature_2m { get; set; }
        public int relative_humidity_2m { get; set; }
        public int is_day { get; set; }
        public double precipitation { get; set; }
        public Enums.WMOCodes weather_code { get; set; }
    }
}