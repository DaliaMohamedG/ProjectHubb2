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
            var message = new Message
            {
                Conversation_ID = messageDto.ConversationId,
                Sender_ID = messageDto.SenderId,
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
                (c.User_ID == currentUserId && c.TargetUser_ID == targetUserId) ||
                (c.User_ID == targetUserId && c.TargetUser_ID == currentUserId));

            if (existingChat != null)
                return existingChat.Id;

            var newChat = new Conversation
            {
                User_ID = currentUserId,
                TargetUser_ID = targetUserId,
                Start_Date = DateTime.Now
            };

            await _unitOfWork.Repository<Conversation>().AddAsync(newChat);
            await _unitOfWork.CompleteAsync();

            return newChat.Id;
        }
    }
}
