namespace DomainLayer.DTOs
{
    public class CommentCreateDto
    {
        public string Content { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
    }
}
