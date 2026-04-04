namespace DomainLayer.DTOs
{
    public class PostResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }

        public int LikesCount { get; set; }
        public bool IsLiked { get; set; }
    }
}
