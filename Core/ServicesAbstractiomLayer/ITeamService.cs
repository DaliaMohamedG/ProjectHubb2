using DomainLayer.DTOs;

namespace ServicesAbstractionLayer
{
    public interface ITeamService
    {
        Task<bool> CreateTeamAsync(CreateTeamDto dto);
        Task<bool> DeleteTeamAsync(int teamId);
        Task<IEnumerable<TeamResponseDto>> GetTeamsByUserIdAsync(string userId);

    }
}
