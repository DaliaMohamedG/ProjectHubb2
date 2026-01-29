namespace DomainLayer.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public string Sender_ID { get; set; }
        public User Sender { get; set; } = null!;
        public int Conversation_ID { get; set; }
        public Conversation Conversation { get; set; }
    }
}
