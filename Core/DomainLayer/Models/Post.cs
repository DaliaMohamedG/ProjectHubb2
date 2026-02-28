using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int CommunityId { get; set; }
        [ForeignKey("CommunityId")]
        public virtual Community Community { get; set; }

        // ميزة الخصوصية: لو نل يبقى عام، لو فيه قيمة يبقى للتيم ده بس
        public int? TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team? Team { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
