using System.Net.Http.Json;
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
            //Setup
            WeatherRetriever weatherRetriever = new();
            weatherRetriever.SetupClient();
            List<string> results = [];

            //Start
            var text = AnsiConsole.Ask<string>("Insert yout city: ");
            text = text.Replace(" ", "+");

            //City Search
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
                        results.Add(singleResult.name! + ", " +
                            singleResult.country + ", " +
                            (singleResult.admin1 != null ? singleResult.admin1 : "-") + ", " +
                            (singleResult.admin2 != null ? singleResult.admin2 : "-") + ", " +
                            (singleResult.admin3 != null ? singleResult.admin3 : "-") + ", " +
                            (singleResult.admin4 != null ? singleResult.admin4 : "-"));
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
            AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Select the correct city")
                .PageSize(10)
                .AddChoices(results)
            );
        }
    }
}
