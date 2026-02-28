using System.ComponentModel.DataAnnotations.Schema;
namespace DomainLayer.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;

        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
    }
}
