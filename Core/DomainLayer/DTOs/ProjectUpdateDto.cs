using Microsoft.AspNetCore.Http;

namespace DomainLayer.DTOs
{
    public class ProjectUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string Category { get; set; }
        public string GitHubUrl { get; set; }
        public IFormFile? CoverPhoto { get; set; }
    }
}
