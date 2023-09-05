using Newtonsoft.Json;

namespace DTOModels
{
    [JsonObject]
    public class PointDto
    {
        [JsonProperty("pointX")]
        public double? pointX { get; set; }

        [JsonProperty("pointY")]
        public double? pointY { get; set; }

        [JsonProperty("color")]
        public string? color { get; set; }
    }
}
