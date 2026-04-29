using DomainLayer.Contracts;
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
        public TeamsController(ITeamService teamService, IUnitOfWork unitOfWork)
        {
            _teamService = teamService;
        }

        [HttpGet("search-users")]
        public async Task<IActionResult> SearchUsers([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("Name cannot be empty");

            var results = await _teamService.SearchUsersAsync(name);
            return Ok(results);
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

        //[HttpGet("members-by-name/{teamName}")]
        //public async Task<IActionResult> GetMembersByName(string teamName)
        //{
        //    var members = await _teamService.GetMembersByTeamNameAsync(teamName);

        //    if (!members.Any())
        //        return NotFound($"No team found with name: {teamName}");

        //    return Ok(members);
        //}

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
        [HttpPost("add-members")]
        public async Task<IActionResult> AddMembers([FromBody] AddTeamMembersDto dto)
        {
            if (dto.MemberIds == null || !dto.MemberIds.Any())
                return BadRequest("Member list cannot be empty.");

            var result = await _teamService.AddMembersToTeamAsync(dto);

            if (!result)
                return BadRequest("No new members were added. They might be already in the team.");

            return Ok(new { Message = "Members added successfully" });
        }
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetTeamDetails(int id)
        {
            var teamDetails = await _teamService.GetTeamDetailsAsync(id);

            if (teamDetails == null)
                return NotFound($"Team with ID {id} not found");

            return Ok(teamDetails);
        }
    }
}
