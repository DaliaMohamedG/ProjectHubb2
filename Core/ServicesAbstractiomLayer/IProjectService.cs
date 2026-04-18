using DomainLayer.DTOs;

namespace ServicesAbstractionLayer
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectResponseDto>> GetAllProjectsAsync(string? search, string? category);
        Task<bool> UploadStudentProjectAsync(StudentProjectCreateDto dto);
        Task<IEnumerable<object>> GetProjectTeamMembersAsync(int projectId);
        Task<IEnumerable<ProjectResponseDto>> GetMyProjectsAsync(string studentId);

    }
}
