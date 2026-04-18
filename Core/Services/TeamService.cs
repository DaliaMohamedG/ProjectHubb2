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

        public async Task<IEnumerable<User>> SearchUsersAsync(string name)
        {
            var users = await _unitOfWork.Repository<User>().FindAsync(u => u.FullName.Contains(name));

            return users.Select(u => new User
            {
                Id = u.Id,
                FullName = u.FullName,
                Profile_Image = u.Profile_Image 
            });
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
        //public async Task<IEnumerable<TeamResponseDto>> GetTeamsByUserIdAsync(string userId)
        //{
        //    var allTeams = await _unitOfWork.Repository<Team>().ListWithSpec(
        //        t => t.SupervisorId == userId || t.Members.Any(m => m.UserId == userId),
        //        new string[] { "Members.User" }
        //    );

        //    return allTeams.Select(t => new TeamResponseDto
        //    {
        //        Id = t.Id.ToString(),
        //        Name = t.TeamName,
        //        ProjectName = t.ProjectName,
        //        Description = t.Description,
        //        CreatedAt = t.CreatedAt,
        //        SupervisorId = t.SupervisorId,

        //        Members = t.Members?.Select(m => new TeamMemberResponseDto
        //        {
        //            Id = m.UserId.ToString(),
        //            Name = m.User?.FullName ?? "Unknown User",
        //            PhotoUrl = m.User?.Profile_Image,
        //            Role = m.RoleInTeam
        //        }).ToList() ?? new List<TeamMemberResponseDto>()
        //    });
        //}

        public async Task<bool> DeleteTeamAsync(int teamId)
        {
            var teamRepo = _unitOfWork.Repository<Team>();
            var team = await teamRepo.GetByIdAsync(teamId);

            if (team == null) return false;

            teamRepo.Delete(team);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0;
        }

        //public async Task<IEnumerable<TeamMemberResponseDto>> GetMembersByTeamNameAsync(string teamName)
        //{
        //    var team = await _unitOfWork.Repository<Team>().ListWithSpec(
        //        t => t.TeamName.Trim().ToLower() == teamName.Trim().ToLower(),
        //        new string[] { "Members.User" }
        //    );

        //    var targetTeam = team.FirstOrDefault();

        //    if (targetTeam == null) return new List<TeamMemberResponseDto>();

        //    return targetTeam.Members.Select(m => new TeamMemberResponseDto
        //    {
        //        Id = m.UserId,
        //        Name = m.User?.FullName ?? "Unknown User",
        //        PhotoUrl = m.User?.Profile_Image,
        //        Role = m.RoleInTeam
        //    }).ToList();
        //}
        public async Task<bool> AddMembersToTeamAsync(AddTeamMembersDto dto)
        {
            var existingMemberIds = (await _unitOfWork.Repository<TeamMember>()
                .FindAsync(tm => tm.TeamId == dto.TeamId))
                .Select(tm => tm.UserId)
                .ToList();

            foreach (var userId in dto.MemberIds)
            {
                if (!existingMemberIds.Contains(userId))
                {
                    var newMember = new TeamMember
                    {
                        TeamId = dto.TeamId,
                        UserId = userId,
                        RoleInTeam = "Member" 
                    };
                    await _unitOfWork.Repository<TeamMember>().AddAsync(newMember);
                }
            }
            return await _unitOfWork.CompleteAsync() > 0;
        }
        public async Task<TeamResponseDto> GetTeamDetailsAsync(int teamId)
        {
            var team = (await _unitOfWork.Repository<Team>().ListWithSpec(
                t => t.Id == teamId,
                new string[] { "Members.User", "Supervisor" }
            )).FirstOrDefault();

            if (team == null) return null;

            var response = new TeamResponseDto
            {
                Id = team.Id.ToString(),
                Name = team.TeamName,
                ProjectName = team.ProjectName,
                Description = team.Description,
                CreatedAt = team.CreatedAt,
                SupervisorId = team.SupervisorId,
                SupervisorName = team.Supervisor?.FullName ?? "Unknown",
                ActiveProjects = 1 
            };

            foreach (var m in team.Members)
            {
                var memberDto = new TeamMemberResponseDto
                {
                    Id = m.UserId,
                    Name = m.User?.FullName ?? "Unknown",
                    PhotoUrl = m.User?.Profile_Image,
                    Role = m.RoleInTeam
                };

                if (m.User?.Role?.ToLower() == "assistant")
                {
                    response.Assistants.Add(memberDto);
                }
                else
                {
                    response.Members.Add(memberDto);
                }
            }

            return response;
        }
    }
}
