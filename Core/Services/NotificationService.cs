using Microsoft.AspNetCore.SignalR;
using PeresentationLayer.Hubs;

namespace ServicesLayer
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(string userId, string title, string message, string type = "info")
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("ReceiveNotification", new
                {
                    title,
                    message,
                    type,
                    createdAt = DateTime.Now
                });
        }
    }
}