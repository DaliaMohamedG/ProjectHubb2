using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class PostComment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int PostId { get; set; }
        public virtual Post Post { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int? ParentCommentId { get; set; }
        public virtual PostComment? ParentComment { get; set; }
        public virtual ICollection<PostComment> Replies { get; set; } = new List<PostComment>();
    }
}
