using DomainLayer.Contracts;
using DomainLayer.Models;
using Microsoft.AspNetCore.SignalR;
using PeresentationLayer.Hubs;
using ServicesAbstractionLayer;

namespace ServicesLayer
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            IUnitOfWork unitOfWork)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
        }

        public async Task SendToUserAsync(
            string userId, string title, string message, string type = "info")
        {
            // 1. احفظي في الداتابيز
            var notification = new NotificationModel
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<NotificationModel>().AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            // 2. ابعتي Real-time لو متصل
            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("ReceiveNotification", new
                {
                    notification.Id,
                    notification.Title,
                    notification.Message,
                    notification.Type,
                    notification.IsRead,
                    notification.CreatedAt
                });
        }

        public async Task<IEnumerable<NotificationModel>> GetUserNotificationsAsync(string userId)
        {
            return await _unitOfWork.Repository<NotificationModel>()
                .ListWithSpec(
                    n => n.UserId == userId,
                    includes: Array.Empty<System.Linq.Expressions.Expression<Func<NotificationModel, object>>>()
                );
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            var unread = await _unitOfWork.Repository<NotificationModel>()
                .FindAsync(n => n.UserId == userId && !n.IsRead);
            return unread.Count();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _unitOfWork.Repository<NotificationModel>()
                .GetByIdAsync(notificationId);

            if (notification == null) return;

            notification.IsRead = true;
            _unitOfWork.Repository<NotificationModel>().Update(notification);
            await _unitOfWork.CompleteAsync();
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var unread = await _unitOfWork.Repository<NotificationModel>()
                .FindAsync(n => n.UserId == userId && !n.IsRead);

            foreach (var notification in unread)
            {
                notification.IsRead = true;
                _unitOfWork.Repository<NotificationModel>().Update(notification);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}