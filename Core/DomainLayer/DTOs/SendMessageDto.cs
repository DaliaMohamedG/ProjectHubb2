namespace DomainLayer.DTOs
{
    public class SendMessageDto
    {
        public string SenderId { get; set; }
        public string TargetUserId { get; set; }
        public string Content { get; set; }
        public int ConversationId { get; set; }
    }
}
