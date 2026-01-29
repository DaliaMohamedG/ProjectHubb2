using DomainLayer.DTOs;

namespace ServicesAbstractionLayer
{
    public interface IConversationService
    {
        Task<bool> SendMessageAsync(SendMessageDto messageDto);
        Task<int> GetOrCreateConversationAsync(string currentUserId, string targetUserId);
    }
}
