using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public DateTime Start_Date { get; set; }
        [ForeignKey("Sender")]
        public string Sender_ID { get; set; }
        public User Sender { get; set; }
        [ForeignKey("TargetUser")]
        public string TargetUser_ID { get; set; }
        public User TargetUser { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
