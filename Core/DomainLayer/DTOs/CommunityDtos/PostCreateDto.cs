using Microsoft.AspNetCore.Http;

namespace DomainLayer.DTOs
{
    public class PostCreateDto
    {
        public string Content { get; set; }
        public string Visibility { get; set; } = "public";
        public string UserId { get; set; }
        public IFormFile? PostImage { get; set; }
        public int? TeamId { get; set; }
    }
}
