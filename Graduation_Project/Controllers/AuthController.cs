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
        public async Task<IActionResult> Sync([FromBody] UserSyncDto dto)
        {
            var result = await _authService.SyncUserAsync(dto);
            if (result) return Ok("Data Synced to SQL Server!");
            return BadRequest("Failed to sync data.");
        }
    }
}
