using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Spectre.Console;

namespace Tenki
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            //Home path
            string userHomePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            //Config path
            string confPath = userHomePath + "/.config/tenki.conf";
            //Setup
            WeatherRetriever weatherRetriever = new();
            weatherRetriever.SetupClient();
            if (!File.Exists(confPath))
            {
                List<(string _string, GeocodingResult _object)> results = [];
                (double latitude, double longittude) coordinates = (0.0, 0.0);

                //City Search
                var text = AnsiConsole.Ask<string>("Insert yout city: ");
                text = text.Replace(" ", "+");
                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots12)
                    .StartAsync("Looking for cities...", async ctx =>
                    {
                        Geocoding? geoInfo = await weatherRetriever.GetGeocodingInfo(text);
                        if (geoInfo == null)
                            return;
                        if (geoInfo.results == null)
                            return;
                        foreach (JsonNode? jn in geoInfo.results)
                        {
                            if (jn == null)
                                continue;
                            GeocodingResult? singleResult = JsonSerializer.Deserialize<GeocodingResult>(jn);
                            if (singleResult == null)
                                return;
                            results.Add((singleResult.name! + ", " +
                                singleResult.country + ", " +
                                (singleResult.admin1 != null ? singleResult.admin1 : "-") + ", " +
                                (singleResult.admin2 != null ? singleResult.admin2 : "-") + ", " +
                                (singleResult.admin3 != null ? singleResult.admin3 : "-") + ", " +
                                (singleResult.admin4 != null ? singleResult.admin4 : "-"), singleResult));
                        }
                        ctx.Refresh();
                    });
                AnsiConsole.Clear();

                // Check
                if (results.Count == 0)
                {
                    AnsiConsole.WriteLine("No city is found with that name [" + text + "]");
                    return;
                }

                // City Selection
                List<string> resultsStrings = [];
                foreach ((string s, GeocodingResult o) x in results)
                {
                    resultsStrings.Add(x.s);
                }
                string chosenCity = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Select the correct city")
                .PageSize(10)
                .AddChoices(resultsStrings));
                coordinates.latitude = results.Find(x => x._string == chosenCity)._object.latitude;
                coordinates.longittude = results.Find(x => x._string == chosenCity)._object.longitude;
                File.Create(confPath).Close();
                File.AppendAllText(confPath, "name=" + text.Replace("+", " ") + Environment.NewLine);
                File.AppendAllText(confPath, "latitude=" + coordinates.latitude + Environment.NewLine);
                File.AppendAllText(confPath, "longitude=" + coordinates.longittude + Environment.NewLine);
            }
            List<string> configs = [.. File.ReadAllLines(confPath)];
            int startIndex = 0;
            string lat = "";
            string lon = "";
            string? latString = configs.Find(x => x.Contains("latitude"));
            string? lonString = configs.Find(x => x.Contains("longitude"));
            string? name = configs.Find(x => x.Contains("name"));
            if (latString == null)
            {
                AnsiConsole.WriteLine("Error with config file [latitude property not found in config file]");
            }
            if (lonString == null)
            {
                AnsiConsole.WriteLine("Error with config file [longitude property not found in config file]");
            }
            if (name == null)
            {
                AnsiConsole.WriteLine("Error with config file [name property not found in config file]");
            }
            startIndex = latString!.IndexOf("=") + 1;
            lat = latString.Substring(startIndex).Replace(",", ".");
            startIndex = lonString!.IndexOf("=") + 1;
            lon = lonString!.Substring(startIndex).Replace(",", ".");
            startIndex = name!.IndexOf("=") + 1;
            name = name.Substring(startIndex).Replace("+", " ");

            WeatherCurrent? weatherCurrent = await weatherRetriever.GetWeather(lat, lon);
            if (weatherCurrent != null)
            {
                AnsiConsole.WriteLine(name+": "+weatherCurrent.temperature_2m+" C, "+weatherCurrent.precipitation);
                // SHOW APP
            }
        }
    }
}
