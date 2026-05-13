using DomainLayer.DTOs;
using DomainLayer.Models;

namespace ServicesAbstractionLayer
{
    public interface ITeamService
    {
        Task<object> CreateTeamAsync(CreateTeamDto dto);
        Task<bool> DeleteTeamAsync(int teamId);
        Task<IEnumerable<TeamResponseDto>> GetTeamsByUserIdAsync(string userId);
        //Task<IEnumerable<TeamMemberResponseDto>> GetMembersByTeamNameAsync(string teamName);
        Task<IEnumerable<User>> SearchUsersAsync(string name);
        Task<object> AddMembersToTeamAsync(AddTeamMembersDto dto);
        Task<TeamResponseDto> GetTeamDetailsAsync(int teamId);
    }
}
