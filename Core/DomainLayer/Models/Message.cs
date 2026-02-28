using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public int Conversation_ID { get; set; }
        [ForeignKey("Conversation_ID")]
        public virtual Conversation Conversation { get; set; }
    }
}
