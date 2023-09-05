using System;
using Newtonsoft.Json;

namespace DTOModels
{
    [JsonObject]
    public class BirthDateDto
    {
        [JsonProperty("birth-date")]
        public string? BirthDate { get; set; }
    }
}

