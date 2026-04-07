using DomainLayer.Contracts;
using DomainLayer.DTOs;
using DomainLayer.Models;
using ServicesAbstractionLayer;

namespace ServicesLayer
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProjectService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<ProjectResponseDto>> GetAllProjectsAsync(string? search, string? category)
        {
            var projects = await _unitOfWork.Repository<Project>().ListWithSpec(
                p => true,
                p => p.Student
            );

            if (!string.IsNullOrEmpty(search))
            {
                projects = projects.Where(p => p.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                                            || p.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                projects = projects.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            return projects.Select(p => new ProjectResponseDto
            {
                Id = p.Id.ToString(),
                Title = p.Title,
                Description = p.Description,
                AuthorId = p.StudentId,
                AuthorName = p.Student?.FullName ?? "Unknown User",
                Category = p.Category,
                Tags = p.TechnologyUsed?.Split(',').Select(t => t.Trim()).ToList() ?? new List<string>(),
                Images = new List<string> { p.ImageUrl },
                GithubUrl = p.ProjectFilePath,
                CreatedAt = p.CreatedAt
            }).OrderByDescending(p => p.CreatedAt);
        }
        public async Task<bool> UploadStudentProjectAsync(StudentProjectCreateDto dto)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            string imagesFolder = Path.Combine(uploadsFolder, "images");
            string docsFolder = Path.Combine(uploadsFolder, "documents");

            if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);
            if (!Directory.Exists(docsFolder)) Directory.CreateDirectory(docsFolder);

            string imageFileName = Guid.NewGuid() + "_" + dto.CoverPhoto.FileName;
            string imagePath = Path.Combine(imagesFolder, imageFileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await dto.CoverPhoto.CopyToAsync(stream);
            }

            string? docFileName = null;
            if (dto.ProjectDocument != null)
            {
                docFileName = Guid.NewGuid() + "_" + dto.ProjectDocument.FileName;
                string docPath = Path.Combine(docsFolder, docFileName);
                using (var stream = new FileStream(docPath, FileMode.Create))
                {
                    await dto.ProjectDocument.CopyToAsync(stream);
                }
            }

            var newProject = new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                TechnologyUsed = dto.Tags,
                StudentId = dto.AuthorId,
                ProjectFilePath = docFileName != null ? "/uploads/documents/" + docFileName : dto.GitHubUrl,
                ImageUrl = "/uploads/images/" + imageFileName,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Project>().AddAsync(newProject);
            return await _unitOfWork.CompleteAsync() > 0;
        }
        public async Task<IEnumerable<object>> GetProjectTeamMembersAsync(int projectId)
        {
            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(projectId);

            if (project == null) return null;

            var memberInfo = await _unitOfWork.Repository<TeamMember>().GetEntityWithSpec(
                m => m.UserId == project.StudentId && m.Team.ProjectName == project.Title,
                m => m.Team.Members
            );

            if (memberInfo?.Team == null) return new List<object>();

            var teamMembers = await _unitOfWork.Repository<TeamMember>().ListWithSpec(
                m => m.TeamId == memberInfo.TeamId,
                m => m.User
            );

            return teamMembers.Select(m => new
            {
                Id = m.User.Id,
                Name = m.User.FullName,
                PhotoUrl = m.User.Profile_Image ?? "default.png",
                Role = m.RoleInTeam
            });
        }
    }
}
