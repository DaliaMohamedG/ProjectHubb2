using Microsoft.AspNetCore.Mvc;
using ServicesLayer;

namespace Graduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
        {
            await _notificationService.SendToUserAsync(
                request.UserId,
                request.Title,
                request.Message,
                request.Type
            );
            return Ok(new { success = true });
        }
    }

    public class SendNotificationRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "info";
    }
}
