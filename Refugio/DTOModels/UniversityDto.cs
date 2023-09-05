using System;
using Newtonsoft.Json;

namespace DTOModels
{
    [JsonObject]
    public class UniversityDto
    {
        [JsonProperty("university")]
        public string? University { get; set; }
    }
}

