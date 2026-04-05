using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Visibility { get; set; } = "Public";

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int? TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team? Team { get; set; }
        public virtual ICollection<Like> Likes { get; set; } = new HashSet<Like>();
        public virtual ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
    }
}
