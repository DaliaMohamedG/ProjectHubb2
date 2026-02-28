using DomainLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractionLayer;

namespace Graduation_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ChatController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var result = await _conversationService.SendMessageAsync(dto);
            if (result) return Ok(new { message = "Message sent successfully!" });
            return BadRequest("Failed to send message.");
        }

        [HttpGet("get-id")]
        public async Task<IActionResult> GetConversationId(string userId, string targetId)
        {
            var id = await _conversationService.GetOrCreateConversationAsync(userId, targetId);
            return Ok(new { conversationId = id });
        }


        [HttpGet("my-conversations/{userId}")]
        public async Task<IActionResult> GetConversations(string userId)
        {
            var conversations = await _conversationService.GetUserConversationsAsync(userId);
            return Ok(conversations);
        }

        [HttpGet("messages/{conversationId}")]
        public async Task<IActionResult> GetMessages(int conversationId)
        {
            var messages = await _conversationService.GetMessagesByConversationIdAsync(conversationId);
            return Ok(messages);
        }
    }
}
