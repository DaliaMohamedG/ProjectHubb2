using DomainLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractionLayer;

namespace Graduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
        {
            if (dto == null) return BadRequest("Data not completed");

            var result = await _teamService.CreateTeamAsync(dto);

            if (result)
                return Ok(new { message = "Team created successfully" });

            return BadRequest(new { message = "Error creating team" });
        }

        [HttpGet("my-teams/{userId}")]
        public async Task<IActionResult> GetMyTeams(string userId)
        {
            var teams = await _teamService.GetTeamsByUserIdAsync(userId);
            return Ok(teams);
        }

        [HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteTeam(int teamId)
        {
            var result = await _teamService.DeleteTeamAsync(teamId);
            if (result) return Ok(new { message = "Team deleted successfully" });

            return NotFound(new { message = "can't find team" });
        }
    }
}
