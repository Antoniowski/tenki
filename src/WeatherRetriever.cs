
using System.Net.Http.Json;
using System.Text.Json;

namespace Tenki
{
    /// <summary>
    /// Class used to retrieve weather and geocoding information using an API Get call
    /// </summary>
    public class WeatherRetriever()
    {
        // PUBLIC
        public bool isReady = false;

        // PRIVATE
        private const string WEATHER_BASE_PATH = "https://api.open-meteo.com";
        private const string GEOCODING_BASE_PATH = "https://geocoding-api.open-meteo.com";
        private const string WEATHER_PATH_1 = "/v1/forecast?latitude=";
        private const string WEATHER_PATH_2 = "&longitude=";
        private const string WEATHER_PATH_3 = "&current=temperature_2m,is_day,precipitation,weather_code";
        private const string GEOCODING_PATH_1 = "/v1/search?name=";
        private const string GEOCODING_PATH_2 = "&count=10&language=en&format=json";
        private HttpClient weatherClient = new HttpClient();
        private HttpClient geocodingClient = new HttpClient();

        // METHODS

        /// <summary>
        /// Setup http clients that will be used to perform Get calls.
        /// To avoid errors this action is performed just once. If the function will 
        /// be called again nothing will happen.
        /// </summary>
        /// 
        /// <returns>
        /// void
        /// </returns>
        public void SetupClient()
        {
            if (!isReady)
            {
                isReady = true;
                Uri weatherUri = new Uri(WEATHER_BASE_PATH);
                Uri geocodingUri = new Uri(GEOCODING_BASE_PATH);
                weatherClient.BaseAddress = weatherUri;
                geocodingClient.BaseAddress = geocodingUri;
            }
        }

        /// <summary>
        /// Get the current weather information
        /// </summary>
        /// 
        /// <returns>
        /// WeatherCurrent - current weather values for temperature, precipitation e weather type
        /// </returns>
        public async Task<WeatherCurrent?> GetWeather(double latitude, double longitude)
        {
            Weather? weatherInfo = new Weather();
            WeatherCurrent? weatherCurrentInfo = new WeatherCurrent();

            if (!isReady)
                return null;

            HttpResponseMessage httpResponseMessage = await weatherClient.GetAsync(WEATHER_PATH_1+latitude+WEATHER_PATH_2+longitude+WEATHER_PATH_3);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                weatherInfo = await httpResponseMessage.Content.ReadFromJsonAsync<Weather>();
                if (weatherInfo == null)
                    return null;
                weatherCurrentInfo = weatherInfo.current.Deserialize<WeatherCurrent>();
                return weatherCurrentInfo;
            }

            return null;
        }


        /// <summary>
        /// Get the geocoding information using the selected city name
        /// </summary>
        /// 
        /// <returns>
        /// WeatherCurrent - current weather values for temperature, precipitation e weather type
        /// </returns>
        public async Task<Geocoding?> GetGeocodingInfo(string cityToSearch)
        {
            Geocoding? geoInfo = new Geocoding();
            string formatted_string = cityToSearch.Replace(" ", "+");
            if (!isReady)
                return null;

            HttpResponseMessage httpResponseMessage = await geocodingClient.GetAsync(GEOCODING_PATH_1 + formatted_string + GEOCODING_PATH_2);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                geoInfo = await httpResponseMessage.Content.ReadFromJsonAsync<Geocoding>();
                if (geoInfo == null)
                    return null;
                Console.Write(geoInfo.results);
                return geoInfo;
            }
            return null;
        }
    }
}
