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
        public async Task<object> CreateTeamAsync(CreateTeamDto dto)
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
                var existingGlobalMembers = (await _unitOfWork.Repository<TeamMember>()
                    .FindAsync(tm => dto.MemberIds.Contains(tm.UserId))).Select(tm => tm.UserId).ToList();

                var studentInfo = await _unitOfWork.Repository<Student>().ListWithSpec(s => dto.MemberIds.Contains(s.Id));

                var skippedStudents = new List<string>();
                foreach (var userId in dto.MemberIds)
                {
                    if (existingGlobalMembers.Contains(userId))
                    {
                        var name = studentInfo.FirstOrDefault(s => s.Id == userId)?.FullName ?? userId;
                        skippedStudents.Add(name);
                        continue;
                    }
                    if (userId == dto.SupervisorId) continue;
                    var studentTrack = studentInfo.FirstOrDefault(s => s.Id == userId)?.Track;

                    var member = new TeamMember
                    {
                        TeamId = team.Id,
                        UserId = userId,
                        RoleInTeam = studentTrack ?? "Assistant"
                    };
                    await _unitOfWork.Repository<TeamMember>().AddAsync(member);
                }
                if (skippedStudents.Any())
                {
                    var message = $"The students were successfully added, except for: {string.Join(", ", skippedStudents)}, they are already enrolled in other teams";
                    return new { success = true, warning = message };
                }
            }

            var finalResult = await _unitOfWork.CompleteAsync();
            if (finalResult > 0) return new { Success = true, Message = "Team created successfully!" };
            return new { Success = false, Message = "Failed to create team." };
        }
        public async Task<IEnumerable<TeamResponseDto>> GetTeamsByUserIdAsync(string userId)
        {
            var allTeams = await _unitOfWork.Repository<Team>().ListWithSpec(
                t => t.SupervisorId == userId || t.Members.Any(m => m.UserId == userId),
                new string[] { "Members.User" }
            );
            var baseUrl = "https://projecthubb.runasp.net";

            return allTeams.Select(t => new TeamResponseDto
            {
                Id = t.Id.ToString(),
                Name = t.TeamName,
                ProjectName = t.ProjectName,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                SupervisorId = t.SupervisorId,
                SupervisorPhotoUrl = string.IsNullOrEmpty(t.Supervisor.Profile_Image) ? null : baseUrl + t.Supervisor.Profile_Image,
                Members = t.Members?
            .Where(m => m.RoleInTeam != "Leader")
            .Select(m => new TeamMemberResponseDto
            {
                Id = m.UserId.ToString(),
                Name = m.User?.FullName ?? "Unknown User",
                PhotoUrl = string.IsNullOrEmpty(m.User.Profile_Image) ? null : baseUrl + m.User.Profile_Image,
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
        public async Task<object> AddMembersToTeamAsync(AddTeamMembersDto dto)
        {
            if (!int.TryParse(dto.TeamId, out int teamIdInt))
            {
                return new { success = false, message = "Invalid Team ID" };
            }

            var existingGlobalMembers = (await _unitOfWork.Repository<TeamMember>()
                .FindAsync(tm => dto.MemberIds.Contains(tm.UserId)))
                .Select(tm => tm.UserId)
                .ToList();

            var studentInfo = await _unitOfWork.Repository<Student>()
                .ListWithSpec(s => dto.MemberIds.Contains(s.Id));

            var skippedStudents = new List<string>();
            int addedCount = 0;

            foreach (var userId in dto.MemberIds)
            {
                if (existingGlobalMembers.Contains(userId))
                {
                    var name = studentInfo.FirstOrDefault(s => s.Id == userId)?.FullName ?? userId;
                    skippedStudents.Add(name);
                    continue;
                }

                var newMember = new TeamMember
                {
                    TeamId = teamIdInt,
                    UserId = userId,
                    RoleInTeam = "Member"
                };

                await _unitOfWork.Repository<TeamMember>().AddAsync(newMember);
                addedCount++;

            }

            var result = await _unitOfWork.CompleteAsync();

            if (skippedStudents.Any())
            {
                string skipMessage = $"Added {addedCount} members. Skipped: {string.Join(", ", skippedStudents)} (already in other teams).";
                return new { success = true, message = skipMessage };
            }

            return new { success = result > 0, message = result > 0 ? "Members added successfully" : "No new members were added" };
        }
        public async Task<TeamResponseDto> GetTeamDetailsAsync(int teamId)
        {
            var team = (await _unitOfWork.Repository<Team>().ListWithSpec(
                t => t.Id == teamId,
                new string[] { "Members.User", "Supervisor" }
            )).FirstOrDefault();

            if (team == null) return null;

            var baseUrl = "https://projecthubb.runasp.net";
            var response = new TeamResponseDto
            {
                Id = team.Id.ToString(),
                Name = team.TeamName,
                ProjectName = team.ProjectName,
                Description = team.Description,
                CreatedAt = team.CreatedAt,
                SupervisorId = team.SupervisorId,
                SupervisorName = team.Supervisor?.FullName ?? "Unknown",
                SupervisorPhotoUrl = string.IsNullOrEmpty(team.Supervisor.Profile_Image) ? null : baseUrl + team.Supervisor.Profile_Image,
                ActiveProjects = 1
            };

            foreach (var m in team.Members)
            {
                var memberDto = new TeamMemberResponseDto
                {
                    Id = m.UserId,
                    Name = m.User?.FullName ?? "Unknown",
                    PhotoUrl = string.IsNullOrEmpty(m.User.Profile_Image) ? null : baseUrl + m.User.Profile_Image,
                    Role = m.RoleInTeam
                };

                if (m.User?.Role?.ToLower() == "assistant")
                    response.Assistants.Add(memberDto);

                else if (m.User?.Role?.ToLower() == "user" || m.User?.Role?.ToLower() == "student")
                    response.Members.Add(memberDto);

            }
            return response;
        }
    }
}
