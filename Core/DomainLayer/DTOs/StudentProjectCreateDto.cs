using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace DomainLayer.DTOs
{
    public class StudentProjectCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; } 
        public string Category { get; set; }

        [JsonPropertyName("githubUrl")]
        public string GitHubUrl { get; set; }
        public IFormFile CoverPhoto { get; set; }

        [JsonPropertyName("documentUrl")] 
        public IFormFile? ProjectDocument { get; set; }

        [JsonPropertyName("authorId")] 
        public string AuthorId { get; set; }
    }
}