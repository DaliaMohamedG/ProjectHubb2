using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllAsync();
        Task<Project?> GetByIdAsync(int id);

        Task<Project?> GetProjectWithDetailsAsync(int id);
        Task<IEnumerable<Project>> GetProjectsByDoctorAsync(int doctorId);
        Task<IEnumerable<Project>> GetProjectsByStudentAsync(int studentId);

        Task AddAsync(Project project);
        void Update(Project project);
        void Delete(Project project);

        Task SaveAsync();
    }
}
