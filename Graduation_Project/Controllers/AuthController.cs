using DomainLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractionLayer;

namespace Graduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromForm] UserSyncDto dto)
        {
            var result = await _authService.SyncUserAsync(dto);
            if (result) return Ok("Data Synced to SQL Server!");
            return BadRequest("Failed to sync data.");
        }
        [HttpGet("profile-image/{userId}")]
        public async Task<IActionResult> GetProfileImage(string userId)
        {
            var imageUrl = await _authService.GetUserProfileImageAsync(userId);

            if (string.IsNullOrEmpty(imageUrl))
            {
                return NotFound(new { message = "User or image not found" });
            }

            return Ok(new { profileImage = imageUrl });
        }
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile([FromForm] UserEditDto dto)
        {
            var result = await _authService.EditUserProfileAsync(dto);

            if (!result)
                return BadRequest(new { message = "Update failed or user not found" });

            return Ok(new { message = "Profile updated successfully" });
        }
    }
}

