using System;
using Newtonsoft.Json;

namespace DTOModels
{
    [JsonObject]
    public class FacultyDto
    {
        [JsonProperty("faculty-name")]
        public string? FacultyName { get; set; }
    }
}

