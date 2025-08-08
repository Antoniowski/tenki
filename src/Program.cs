using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Spectre.Console;

namespace Tenki{
    public static class Program{
        public static async Task Main(string[] args)
        {
            WeatherRetriever weatherRetriever = new WeatherRetriever();
            weatherRetriever.SetupClient();
            
            Geocoding? geo = await weatherRetriever.GetGeocodingInfo("Somma Vesuviana");
        }
    }
}
