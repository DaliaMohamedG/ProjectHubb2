using DomainLayer.Contracts;
using DomainLayer.DTOs;
using DomainLayer.Models;
using ServicesAbstractionLayer;

namespace ServicesLayer
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public TeamService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
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
            var skippedStudents = new List<string>();
            var studentsTrackInfo = new List<Student>();
            var alreadyEnrolledStudentIds = new List<string>();
            var usersInfo = new List<User>();

            if (dto.MemberIds != null && dto.MemberIds.Any())
            {
                usersInfo = (await _unitOfWork.Repository<User>().FindAsync(u => dto.MemberIds.Contains(u.Id))).ToList();

                var studentIdsInDto = usersInfo
                    .Where(u => u.Role == "student" || u.Role == "user")
                    .Select(u => u.Id)
                    .ToList();

                if (studentIdsInDto.Any())
                {
                    alreadyEnrolledStudentIds = (await _unitOfWork.Repository<TeamMember>()
                        .FindAsync(tm => studentIdsInDto.Contains(tm.UserId)))
                        .Select(tm => tm.UserId)
                        .Distinct()
                        .ToList();

                    studentsTrackInfo = (await _unitOfWork.Repository<Student>().FindAsync(s => studentIdsInDto.Contains(s.Id))).ToList();
                }
            }

            var team = new Team
            {
                TeamName = dto.Name,
                ProjectName = dto.ProjectName,
                Description = dto.Description,
                SupervisorId = dto.SupervisorId,
                CreatedAt = DateTime.UtcNow
            };

            var supervisorMember = new TeamMember
            {
                UserId = dto.SupervisorId,
                RoleInTeam = "Leader"
            };
            team.Members.Add(supervisorMember);

            if (dto.MemberIds != null && dto.MemberIds.Any())
            {
                foreach (var userId in dto.MemberIds)
                {
                    if (userId == dto.SupervisorId) continue;

                    var user = usersInfo.FirstOrDefault(u => u.Id == userId);
                    if (user == null) continue;

                    if ((user.Role == "student" || user.Role == "user") && alreadyEnrolledStudentIds.Contains(userId))
                    {
                        skippedStudents.Add(user.FullName ?? userId);
                        continue;
                    }

                    string roleInTeam = "Assistant";
                    if (user.Role == "student" || user.Role == "user")
                    {
                        roleInTeam = studentsTrackInfo.FirstOrDefault(s => s.Id == userId)?.Track ?? "Student";
                    }

                    var member = new TeamMember
                    {
                        UserId = userId,
                        RoleInTeam = roleInTeam
                    };

                    team.Members.Add(member);
                }
            }

            await _unitOfWork.Repository<Team>().AddAsync(team);

            var finalResult = await _unitOfWork.CompleteAsync();

            if (finalResult > 0)
            {
                if (dto.MemberIds != null)
                {
                    foreach (var userId in dto.MemberIds)
                    {
                        if (userId == dto.SupervisorId || alreadyEnrolledStudentIds.Contains(userId)) continue;
                        try
                        {
                            await _notificationService.SendToUserAsync(userId, "New team created!", $"You have been added to {dto.Name}", "info");
                        }
                        catch { }
                    }
                }

                if (skippedStudents.Any())
                {
                    var warningMessage = $"The team was successfully created, except for: {string.Join(", ", skippedStudents)}, they are already enrolled in other teams.";
                    return new { Success = true, Warning = warningMessage };
                }

                return new { Success = true, Message = "Team created successfully!" };
            }

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

            var team = await _unitOfWork.Repository<Team>().GetByIdAsync(teamIdInt);
            if (team == null)
            {
                return new { success = false, message = "Team not found" };
            }

            var currentTeamMemberIds = (await _unitOfWork.Repository<TeamMember>()
                .FindAsync(tm => tm.TeamId == teamIdInt))
                .Select(tm => tm.UserId)
                .ToList();

            var usersInfo = await _unitOfWork.Repository<User>()
                .ListWithSpec(u => dto.MemberIds.Contains(u.Id));

            var studentIdsInDto = usersInfo
                .Where(u => u.Role == "student" || u.Role == "user")
                .Select(u => u.Id)
                .ToList();

            var alreadyEnrolledStudentIds = (await _unitOfWork.Repository<TeamMember>()
                .FindAsync(tm => studentIdsInDto.Contains(tm.UserId)))
                .Select(tm => tm.UserId)
                .Distinct()
                .ToList();

            var studentsTrackInfo = await _unitOfWork.Repository<Student>()
                .ListWithSpec(s => studentIdsInDto.Contains(s.Id));

            var skippedStudents = new List<string>();
            int addedCount = 0;

            var newlyAddedIds = new List<string>();

            foreach (var userId in dto.MemberIds)
            {
                var user = usersInfo.FirstOrDefault(u => u.Id == userId);
                if (user == null) continue;

                if (currentTeamMemberIds.Contains(userId) || newlyAddedIds.Contains(userId)) continue;

                if ((user.Role == "student" || user.Role == "user") && alreadyEnrolledStudentIds.Contains(userId))
                {
                    var name = user.FullName ?? userId;
                    skippedStudents.Add(name);
                    continue;
                }

                string roleInTeam = "Member";
                if (user.Role == "student" || user.Role == "user")
                {
                    roleInTeam = studentsTrackInfo.FirstOrDefault(s => s.Id == userId)?.Track ?? "Student";
                }
                else
                {
                    roleInTeam = "Assistant";
                }

                var newMember = new TeamMember
                {
                    TeamId = teamIdInt,
                    UserId = userId,
                    RoleInTeam = roleInTeam
                };

                await _unitOfWork.Repository<TeamMember>().AddAsync(newMember);
                newlyAddedIds.Add(userId);

                await _notificationService.SendToUserAsync(
                    userId,
                    "You have been added to a team",
                    $"You have been added to {team.TeamName}",
                    "info"
                );

                addedCount++;
            }

            var result = await _unitOfWork.CompleteAsync();

            if (skippedStudents.Any())
            {
                string skipMessage = $"Added {addedCount} members. Skipped: {string.Join(", ", skippedStudents)} (already in other teams).";
                return new { success = true, message = skipMessage };
            }

            return new { success = result > 0 || addedCount > 0, message = addedCount > 0 ? "Members added successfully" : "No new members were added" };
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
