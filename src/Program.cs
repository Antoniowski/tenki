using System.Net.Http.Json;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Tenki
{
    public static class Program
    {
        public static WeatherRetriever? weatherRetriever;
        public static WeatherCurrent? weatherCurrent;

        public static void OnRetrievedWeather()
        {
            if (weatherRetriever == null)
                return;
            weatherCurrent = weatherRetriever.weatherCurrent;
        }

        public static async Task Main(string[] args)
        {
            //Home path
            string userHomePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            //Config path
            string confPath = userHomePath + "/.config/tenki.conf";
            //Setup
            WeatherRetriever weatherRetriever = new();
            weatherRetriever.SetupClient();
            weatherRetriever.RetrievedWeather += OnRetrievedWeather;

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
                    resultsStrings.Add(x.s);

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

            //App
            List<string> configs = [.. File.ReadAllLines(confPath)];
            int startIndex = 0;
            string lat = "";
            string lon = "";
            string? latString = configs.Find(x => x.Contains("latitude"));
            string? lonString = configs.Find(x => x.Contains("longitude"));
            string? name = configs.Find(x => x.Contains("name"));

            if (latString == null)
                AnsiConsole.WriteLine("Error with config file [latitude property not found in config file]");
            if (lonString == null)
                AnsiConsole.WriteLine("Error with config file [longitude property not found in config file]");
            if (name == null)
                AnsiConsole.WriteLine("Error with config file [name property not found in config file]");

            startIndex = latString!.IndexOf("=") + 1;
            lat = latString.Substring(startIndex).Replace(",", ".");
            startIndex = lonString!.IndexOf("=") + 1;
            lon = lonString!.Substring(startIndex).Replace(",", ".");
            startIndex = name!.IndexOf("=") + 1;
            name = name.Substring(startIndex).Replace("+", " ");

            //First weather retrieving
            await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots12)
            .StartAsync("Loading...", async ctx =>
            {
                weatherCurrent = await weatherRetriever.GetWeather(lat, lon);
                if (weatherCurrent == null)
                    return;
                ctx.Refresh();
            });

            while (true)
            {
                while (weatherRetriever.updateWeatherTimer > 0)
                {
                    //Setup canvas
                    Canvas canvas = new(25, 16);
                    ImageMaker imageMaker = new();
                    canvas.PixelWidth = 2;
                    //Setup layout
                    Layout layout = new Layout("Root");
                    layout.SplitColumns(new Layout("Main"));

                    //Draw
                    if (weatherCurrent != null)
                    {
                        canvas = imageMaker.Draw(canvas, weatherCurrent.weather_code, weatherCurrent.is_day == 1 ? true : false);
                        Rows rows = new Rows(new Text(name), canvas, new Text("Temp: " + weatherCurrent.temperature_2m + "°C | Hum: " + weatherCurrent.relative_humidity_2m + "% | P: " + weatherCurrent.precipitation + "%"));
                        AnsiConsole.Clear();
                        layout["Main"].Update(new Align(rows, HorizontalAlignment.Center, VerticalAlignment.Middle));
                        AnsiConsole.Write(layout);
                    }
                    Thread.Sleep(1000);  // Update the canvas every second
                    weatherRetriever.updateWeatherTimer -= 1000;
                }
                weatherRetriever.updateWeatherTimer = 900000;
                weatherCurrent = await weatherRetriever.GetWeather(lat, lon);
            }
        }
    }
}

