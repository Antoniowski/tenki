using System.Text.Json.Nodes;

namespace Tenki {
    public class Weather()
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double generetiontime_ms { get; set; }
        public int utc_offset_seconds { get; set; }
        public string? timezone { get; set; }
        public string? timezone_abbreviation { get; set; }
        public double elevation { get; set; }
        public JsonObject? current_units { get; set; }
        public JsonObject? current { get; set; }
    }
}