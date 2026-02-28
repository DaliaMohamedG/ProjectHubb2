using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public DateTime Start_Date { get; set; }

        // ربط المحادثة بالمشروع
        public int? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        public string Sender_ID { get; set; }
        [ForeignKey("Sender_ID")]
        public virtual User Sender { get; set; }

        public string TargetUser_ID { get; set; }
        [ForeignKey("TargetUser_ID")]
        public virtual User TargetUser { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
