namespace DomainLayer.DTOs.CommunityDtos
{
    public class CommentResponseDto
    {
        public string Id { get; set; }
        public string? PostId { get; set; }
        public string? TaskId { get; set; }
        public string UserName { get; set; }
        public string? UserImage { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Likes { get; set; }
        public bool IsLiked { get; set; }
        public List<CommentResponseDto> Replies { get; set; } = new List<CommentResponseDto>();
    }
}
