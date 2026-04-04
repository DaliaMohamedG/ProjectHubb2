using Microsoft.AspNetCore.Http;

namespace DomainLayer.DTOs
{
    public class PostCreateDto
    {
        public string Content { get; set; }
        public string Visibility { get; set; } = "Public";
        public string UserId { get; set; }
        public IFormFile? PostImage { get; set; }
    }
}
