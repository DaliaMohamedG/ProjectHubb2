using DomainLayer.DTOs;
using DomainLayer.Models;

namespace ServicesAbstractionLayer
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<bool> UploadStudentProjectAsync(StudentProjectCreateDto dto);
    }
}
