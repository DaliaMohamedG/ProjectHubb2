using System.Text.Json.Serialization;

namespace DomainLayer.DTOs
{
    public class UserDto
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; }
    }
}
