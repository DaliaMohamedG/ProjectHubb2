using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.CommunityDtos
{
    public class CommentResponseDto
    {
        public string Id { get; set; } = null!;
        public string? PostId { get; set; }
        public string? TaskId { get; set; }
        public string UserName { get; set; } = null!;
        public string UserInitial { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int Likes { get; set; }
        public bool IsLiked { get; set; }
    }
}
