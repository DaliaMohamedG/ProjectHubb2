using System.Text.Json.Serialization;
namespace DomainLayer.DTOs
{
    public class UserSyncDto
    {
        [JsonPropertyName("uid")]
        public string Id { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("university")]
        public string Instituation { get; set; }

        [JsonPropertyName("faculty")]
        public string Faculty { get; set; }

        [JsonPropertyName("track")]
        public string? Track { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}
