using System;
using Newtonsoft.Json;

namespace DTOModels
{
    [JsonObject]
    public class UserDto
    {

        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("activity")]
        public string? Activity { get; set; }

        [JsonProperty("university")]
        public string? University { get; set; }

        [JsonProperty("facultyName")]
        public string? FacultyName { get; set; }

        [JsonProperty("vkIdString")]
        public string? VkIdString { get; set; }
    }
}

