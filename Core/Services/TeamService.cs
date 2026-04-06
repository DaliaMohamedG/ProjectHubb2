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
                TeamName = dto.Name,
                ProjectName = dto.ProjectName,
                Description = dto.Description,
                SupervisorId = dto.SupervisorId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Team>().AddAsync(team);

            var teamResult = await _unitOfWork.CompleteAsync();
            if (teamResult <= 0) return false;

            var supervisorMember = new TeamMember
            {
                TeamId = team.Id,
                UserId = dto.SupervisorId,
                RoleInTeam = "Leader"
            };
            await _unitOfWork.Repository<TeamMember>().AddAsync(supervisorMember);

            if (dto.MemberIds != null && dto.MemberIds.Any())
            {
                foreach (var userId in dto.MemberIds)
                {
                    if (userId == dto.SupervisorId) continue;

                    var member = new TeamMember
                    {
                        TeamId = team.Id,
                        UserId = userId,
                        RoleInTeam = "Member"
                    };
                    await _unitOfWork.Repository<TeamMember>().AddAsync(member);
                }
            }

            var finalResult = await _unitOfWork.CompleteAsync();
            return finalResult > 0;
        }
        public async Task<IEnumerable<TeamResponseDto>> GetTeamsByUserIdAsync(string userId)
        {
            var supervisedTeams = await _unitOfWork.Repository<Team>().ListWithSpec(
                t => t.SupervisorId == userId,
                t => t.Members
            );

            var memberShips = await _unitOfWork.Repository<TeamMember>().ListWithSpec(
                tm => tm.UserId == userId,
                tm => tm.Team.Members
            );

            var asMemberTeams = memberShips.Select(tm => tm.Team).Where(t => t != null);

            var allTeams = supervisedTeams.Union(asMemberTeams).DistinctBy(t => t.Id);

            return allTeams.Select(t => new TeamResponseDto
            {
                Id = t.Id.ToString(),
                Name = t.TeamName,
                ProjectName = t.ProjectName,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                SupervisorId = t.SupervisorId,

                Members = t.Members?.Select(m => new TeamMemberResponseDto
                {
                    Id = m.UserId,
                    Name = m.User?.FullName ?? "Unknown User",
                    PhotoUrl = m.User?.Profile_Image,
                    Role = m.RoleInTeam
                }).ToList() ?? new List<TeamMemberResponseDto>()
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
