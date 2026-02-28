using DomainLayer.Contracts;
using DomainLayer.DTOs;
using DomainLayer.Models;
using ServicesAbstractionLayer;

namespace ServicesLayer
{
    public class ConversationService : IConversationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ConversationService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> SendMessageAsync(SendMessageDto messageDto)
        {
            int conversationId = await GetOrCreateConversationAsync(messageDto.SenderId, messageDto.TargetUserId);

            var message = new Message
            {
                Conversation_ID = conversationId,
                UserID = messageDto.SenderId,
                Content = messageDto.Content,
                Timestamp = DateTime.Now
            };

            await _unitOfWork.Repository<Message>().AddAsync(message);
            var result = await _unitOfWork.CompleteAsync();

            return result > 0;
        }

        public async Task<int> GetOrCreateConversationAsync(string currentUserId, string targetUserId)
        {
            var conversations = await _unitOfWork.Repository<Conversation>().GetAllAsync();

            var existingChat = conversations.FirstOrDefault(c =>
                (c.Sender_ID == currentUserId && c.TargetUser_ID == targetUserId) ||
                (c.Sender_ID == targetUserId && c.TargetUser_ID == currentUserId));

            if (existingChat != null)
                return existingChat.Id;

            var newChat = new Conversation
            {
                Sender_ID = currentUserId,
                TargetUser_ID = targetUserId,
                Start_Date = DateTime.Now
            };

            await _unitOfWork.Repository<Conversation>().AddAsync(newChat);
            await _unitOfWork.CompleteAsync();

            return newChat.Id;
        }
        public async Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(int conversationId)
        {
            var messages = await _unitOfWork.Repository<Message>()
                .FindAsync(m => m.Conversation_ID == conversationId);

            return messages.OrderBy(m => m.Timestamp);
        }
        public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId)
        {
            var conversations = await _unitOfWork.Repository<Conversation>()
                .FindAsync(c => c.Sender_ID == userId || c.TargetUser_ID == userId);

            return conversations.OrderByDescending(c => c.Start_Date);
        }
    }
}
