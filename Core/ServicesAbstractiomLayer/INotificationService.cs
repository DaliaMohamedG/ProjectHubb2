using DomainLayer.Models;

namespace ServicesAbstractionLayer
{
    public interface INotificationService
    {
        Task SendToUserAsync(string userId, string title, string message, string type = "info");
        Task<IEnumerable<NotificationModel>> GetUserNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
    }
}