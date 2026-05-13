namespace DomainLayer.DTOs.CommunityDtos
{
    public class CommentCreateDto
    {
        public string Content { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}
