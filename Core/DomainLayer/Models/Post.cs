using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        [StringLength(500)]
        public PostStatus Status { get; set; }
        [ForeignKey("Admin")]
        public string AdminId { get; set; }
        public Admin Admin { get; set; }
        [ForeignKey("Community")]
        public int CommunityId { get; set; }
        public Community Community { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }

    }
}
