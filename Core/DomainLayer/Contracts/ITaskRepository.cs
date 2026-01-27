using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TeamTasks>> GetAllAsync();
        Task<TeamTasks?> GetByIdAsync(int id);
        Task<IEnumerable<TeamTasks>> GetTasksByProjectAsync(int projectId);
        Task<IEnumerable<TeamTasks>> GetTasksByStudentAsync(int studentId);
        Task<IEnumerable<TeamTasks>> GetTasksByDoctorAsync(int doctorId);
        Task AddAsync(TeamTasks task);
        void UpdateStatus(TeamTasks task);
        void Delete(TeamTasks task);
        TeamTasks SaveAsync();
    }
}
