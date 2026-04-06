using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace DomainLayer.DTOs
{
    public class ProjectUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public string? Category { get; set; }

        [JsonPropertyName("githubUrl")]
        public string? GitHubUrl { get; set; }

        public IFormFile? CoverPhoto { get; set; }
        public string? Id { get; set; }
    }
}