using DomainLayer.Contracts;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace PersistenceLayer.Repositories
{
    public class TaskRepository : GenericRepository<TeamTasks>, ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────────────
        // Get all tasks for a specific team
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TeamTasks>> GetTasksByTeamIdAsync(int teamId)
        {
            return await _context.Tasks
                .Include(t => t.Student)        
                .Include(t => t.Comments)       
                    .ThenInclude(c => c.User)   
                .Where(t => t.TeamId == teamId)
                .OrderBy(t => t.Deadline)       
                .ToListAsync();
        }

        // ─────────────────────────────────────────────────────
        // Get all tasks assigned to a specific student
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TeamTasks>> GetTasksByStudentIdAsync(string studentId)
        {
            return await _context.Tasks
                .Include(t => t.Team)           
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Where(t => t.AssignedStudentId == studentId)
                .OrderBy(t => t.Deadline)
                .ToListAsync();
        }

        // ─────────────────────────────────────────────────────
        // Get all tasks across all teams of a supervisor
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TeamTasks>> GetTasksBySupervisorIdAsync(string supervisorId)
        {
            return await _context.Tasks
                .Include(t => t.Team)           
                .Include(t => t.Student)        
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Where(t => t.Team.SupervisorId == supervisorId)
                .OrderBy(t => t.Deadline)
                .ToListAsync();
        }

        // ─────────────────────────────────────────────────────
        // Get all tasks where deadline passed and still not done
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TeamTasks>> GetOverdueTasksAsync()
        {
            return await _context.Tasks
                .Include(t => t.Team)
                .Include(t => t.Student)
                .Where(t => t.Deadline < DateTime.UtcNow && t.Status != "Done")
                .OrderBy(t => t.Deadline)
                .ToListAsync();
        }

        // ─────────────────────────────────────────────────────
        // Get tasks for a team filtered by status
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TeamTasks>> GetTasksByTeamAndStatusAsync(int teamId, string status)
        {
            return await _context.Tasks
                .Include(t => t.Student)
                .Where(t => t.TeamId == teamId && t.Status == status)
                .OrderBy(t => t.Deadline)
                .ToListAsync();
        }
    }
    }
