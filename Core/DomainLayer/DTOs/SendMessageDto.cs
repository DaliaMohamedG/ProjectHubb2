namespace DomainLayer.DTOs
{
    public class SendMessageDto
    {
        public int ConversationId { get; set; }
        public string SenderId { get; set; }
        public string Content { get; set; }
    }
}
