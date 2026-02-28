using DomainLayer.DTOs;
using DomainLayer.Models;

namespace ServicesAbstractionLayer
{
    public interface IConversationService
    {
        Task<bool> SendMessageAsync(SendMessageDto messageDto);
        Task<int> GetOrCreateConversationAsync(string currentUserId, string targetUserId);
        Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(int conversationId);
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId);
    }
}
