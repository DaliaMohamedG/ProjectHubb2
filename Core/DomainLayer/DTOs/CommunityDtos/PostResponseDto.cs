namespace DomainLayer.DTOs.PostDtos
{
    public class PostResponseDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
       
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserAvatarColor { get; set; } = "#DBEAFE";
        public List<string> Hashtags { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string Visibility { get; set; } = "public";
        public string? AttachmentName { get; set; }
        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }
        public bool IsLiked { get; set; }
        public List<string> LikedByUserIds { get; set; } = new();
    }
}
