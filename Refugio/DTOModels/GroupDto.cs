using System;
using Newtonsoft.Json;

namespace DTOModels
{
    [JsonObject]
	public class GroupDto
	{
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("activity")]
        public string? Activity { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("member-count")]
        public string? MembersCount { get; set; }

        [JsonProperty("place")]
        public string? Place { get; set; }

        [JsonProperty("is-closed")]
        public bool? IsClosed { get; set; }
    }
}

