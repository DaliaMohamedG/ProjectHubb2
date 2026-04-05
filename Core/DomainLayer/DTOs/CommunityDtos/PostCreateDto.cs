using Microsoft.AspNetCore.Http;

namespace DomainLayer.DTOs.PostDtos
{
    public class PostCreateDto
    {
        public string Content { get; set; }
        public string Visibility { get; set; } = "Public";
        public int? TeamId { get; set; }          // only if visibility = "myTeam"
        public string? AttachmentName { get; set; }
        public IFormFile? PostImage { get; set; }
    }
}
