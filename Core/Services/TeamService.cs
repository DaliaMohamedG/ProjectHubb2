using DomainLayer.Contracts;
using DomainLayer.DTOs;
using DomainLayer.Models;
using ServicesAbstractionLayer;

namespace ServicesLayer
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateTeamAsync(CreateTeamDto dto)
        {
            var team = new Team
            {
                TeamName = dto.TeamName,
                ProjectName = dto.ProjectName,
                Description = dto.Description,
                SupervisorId = dto.SupervisorId,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<Team>().AddAsync(team);

            await _unitOfWork.CompleteAsync();

            foreach (var userId in dto.MemberIds)
            {
                var member = new TeamMember
                {
                    TeamId = team.Id,
                    UserId = userId,
                    RoleInTeam = "Member"
                };
                await _unitOfWork.Repository<TeamMember>().AddAsync(member);
            }

            var result = await _unitOfWork.CompleteAsync();
            return result > 0;
        }
        public async Task<IEnumerable<TeamResponseDto>> GetTeamsByUserIdAsync(string userId)
        {
            var supervisedTeams = await _unitOfWork.Repository<Team>().ListWithSpec(
                t => t.SupervisorId == userId,
                t => t.Members
            );

            var memberShips = await _unitOfWork.Repository<TeamMember>().ListWithSpec(
                tm => tm.UserId == userId,
                tm => tm.Team,
                tm => tm.Team.Members 
            );
            var asMemberTeams = memberShips.Select(tm => tm.Team);

            var allTeams = supervisedTeams.Union(asMemberTeams).DistinctBy(t => t.Id);

            return allTeams.Select(t => new TeamResponseDto
            {
                Id = t.Id,
                TeamName = t.TeamName,
                ProjectName = t.ProjectName,
                CreatedAt = t.CreatedAt,
                SupervisorId = t.SupervisorId,

                Members = t.Members?.Select(m => new UserDto
                {
                    Id = m.UserId,
                    UserName = m.User?.FullName,
                    ProfileImage = m.User?.Profile_Image
                }).ToList() ?? new List<UserDto>()
            });
        }
        public async Task<bool> DeleteTeamAsync(int teamId)
        {
            var teamRepo = _unitOfWork.Repository<Team>();
            var team = await teamRepo.GetByIdAsync(teamId);

            if (team == null) return false;

            teamRepo.Delete(team);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0;
        }
    }
}
