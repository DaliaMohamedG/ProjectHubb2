namespace DomainLayer.DTOs
{
    public class CommentCreateDto
    {
        public string Content { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}
