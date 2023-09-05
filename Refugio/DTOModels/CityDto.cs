using System;
using Newtonsoft.Json;

namespace DTOModels
{
    [JsonObject]
    public class CityDto
    {
        [JsonProperty("city")]
        public string? City { get; set; }
    }
}

