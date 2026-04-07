using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace DomainLayer.DTOs
{
    public class PostResponseDto
    {
        public string Id { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        [JsonPropertyName("user_avatar_color")]
        public string UserAvatarColor { get; set; } = "#DBEAFE";

        [JsonPropertyName("time_ago")]
        public string TimeAgo { get; set; }

        public string Content { get; set; }

        public List<string> Hashtags { get; set; } = new();

        public int Likes { get; set; }

        [JsonPropertyName("comments_count")]
        public int CommentsCount { get; set; }

        [JsonPropertyName("attachment_name")]
        public string? AttachmentName { get; set; }

        [JsonPropertyName("is_liked")]
        public bool IsLiked { get; set; }

        [JsonPropertyName("liked_by_user_ids")]
        public List<string> LikedByUserIds { get; set; } = new();
        public IFormFile? PostImage { get; set; }
        public string Visibility { get; set; } = "public";
    }
}
