using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface ITaskRepository : IGenericRepository<TeamTasks>
    {
        // Get all tasks for a specific team
        // Used in: "Team A Tasks" screen (Supervisor)
        Task<IEnumerable<TeamTasks>> GetTasksByTeamIdAsync(int teamId);

        // Get all tasks assigned to a specific student
        // Used in: "My Work" screen (Student)
        Task<IEnumerable<TeamTasks>> GetTasksByStudentIdAsync(string studentId);

        // Get all tasks across ALL teams of a supervisor
        // Used in: "All Tasks" screen (Supervisor)
        Task<IEnumerable<TeamTasks>> GetTasksBySupervisorIdAsync(string supervisorId);

        // Get tasks that are overdue (deadline passed + not done)
        // Used in: highlighting overdue tasks in any screen
        Task<IEnumerable<TeamTasks>> GetOverdueTasksAsync();

        // Get tasks filtered by status ("Pending" or "Done")
        // Used in: "In Progress" and "Completed" sections in UI
        Task<IEnumerable<TeamTasks>> GetTasksByTeamAndStatusAsync(int teamId, string status);
    }
}
