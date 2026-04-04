namespace DomainLayer.DTOs
{
    public class CommentCreateDto
    {
        public string Text { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
    }
}
