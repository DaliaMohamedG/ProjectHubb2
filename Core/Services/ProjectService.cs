using DomainLayer.Contracts;
using DomainLayer.DTOs;
using DomainLayer.Models;
using ServicesAbstractionLayer;
using SixLabors.ImageSharp;

namespace ServicesLayer
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
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

            return projects.Select(p => MapToProjectResponseDto(p)).OrderByDescending(p => p.CreatedAt);
        }
        public async Task<IEnumerable<ProjectResponseDto>> GetMyProjectsAsync(string studentId)
        {
            var userMemberships = await _unitOfWork.Repository<TeamMember>()
                .ListWithSpec(tm => tm.UserId == studentId, tm => tm.Team);

            var projectNamesFromTeams = userMemberships
                .Select(tm => tm.Team?.ProjectName)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();

            var projects = await _unitOfWork.Repository<Project>()
                .ListWithSpec(
                    p => p.StudentId == studentId || projectNamesFromTeams.Contains(p.Title),
                    p => p.Student
                );

            return projects.Select(p => MapToProjectResponseDto(p)).OrderByDescending(p => p.CreatedAt);
        }
        public async Task<bool> UploadStudentProjectAsync(StudentProjectCreateDto dto)
        {
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "uploads", "images");
            string docsFolder = Path.Combine(wwwroot, "uploads", "documents");

            if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);
            if (!Directory.Exists(docsFolder)) Directory.CreateDirectory(docsFolder);
            List<string> savedImagePaths = new List<string>();

            if (dto.CoverPhoto != null && dto.CoverPhoto.Any())
            {
                foreach (var file in dto.CoverPhoto)
                {
                    string fileName = $"{Guid.NewGuid()}.jpg";
                    string path = Path.Combine(imagesFolder, fileName);

                    using (var inputStream = file.OpenReadStream())
                    {
                        using (var image = await Image.LoadAsync(inputStream))
                        {
                            await image.SaveAsJpegAsync(path);
                        }
                    }
                    savedImagePaths.Add($"/uploads/images/{fileName}");
                }
            }

            string? docFileName = null;
            if (dto.ProjectDocument != null)
            {
                docFileName = $"{Guid.NewGuid()}_{dto.ProjectDocument.FileName}";
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
                ImageUrl = string.Join(",", savedImagePaths),
                ProjectFilePath = docFileName != null ? $"/uploads/documents/{docFileName}" : null,
                GithubUrl = dto.GitHubUrl,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<Project>().AddAsync(newProject);
            var result = await _unitOfWork.CompleteAsync();

            return result > 0;
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
            var baseUrl = "https://projecthubb.runasp.net";
            return teamMembers.Select(m => new
            {
                Id = m.User.Id,
                Name = m.User.FullName,
                PhotoUrl = string.IsNullOrEmpty(m.User.Profile_Image) ? null : baseUrl + m.User.Profile_Image,
                Role = m.RoleInTeam
            });
        }
        public async Task<ProjectResponseDto?> GetProjectByIdAsync(int id)
        {
            var project = await _unitOfWork.Repository<Project>().GetEntityWithSpec(
                p => p.Id == id,
                p => p.Student
            );

            if (project == null) return null;

            return MapToProjectResponseDto(project);
        }
        public async Task<bool> DeleteProject(int id, string studentId)
        {
            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(id);
            if (project == null || project.StudentId != studentId)
                return false;

            _unitOfWork.Repository<Project>().Delete(project);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0;
        }
        private ProjectResponseDto MapToProjectResponseDto(Project p)
        {
            var baseUrl = "https://projecthubb.runasp.net";
            return new ProjectResponseDto
            {
                Id = p.Id.ToString(),
                Title = p.Title,
                Description = p.Description,
                AuthorId = p.StudentId,
                AuthorName = p.Student?.FullName ?? "Unknown User",
                UserImage = string.IsNullOrEmpty(p.Student?.Profile_Image) ? null : (p.Student.Profile_Image.StartsWith("http") ? p.Student.Profile_Image : baseUrl + (p.Student.Profile_Image.StartsWith("/") ? "" : "/") + p.Student.Profile_Image),
                Category = p.Category,
                Tags = p.TechnologyUsed?.Split(',').Select(t => t.Trim()).ToList() ?? new List<string>(),
                Images = string.IsNullOrEmpty(p.ImageUrl) ? new List<string>() : p.ImageUrl.Split(',')
                .Select(img => $"{baseUrl}{(img.StartsWith("/") ? "" : "/")}{img}").ToList(),
                GithubUrl = p.GithubUrl,
                CreatedAt = p.CreatedAt,
                DocumentUrl = string.IsNullOrEmpty(p.ProjectFilePath) ? null : baseUrl + (p.ProjectFilePath.StartsWith("/") ? "" : "/") + p.ProjectFilePath
            };
        }
    }
}
