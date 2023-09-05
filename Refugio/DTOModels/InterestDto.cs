using System;
using Newtonsoft.Json;

namespace DTOModels
{
    [JsonObject]
    public class InterestDto
    {
        [JsonProperty("activities")]
        public string? Activity { get; set; }
    }
}

