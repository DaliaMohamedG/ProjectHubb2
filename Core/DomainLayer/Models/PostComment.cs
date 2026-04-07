using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class PostComment
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string PostId { get; set; }
        public virtual Post Post { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
