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
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.Student)
                .Include(t => t.Comments)       
                    .ThenInclude(c => c.User)   
                .Where(t => t.TeamId == teamId)
                .OrderBy(t => t.DueDate)       
                .ToListAsync();
        }

        // ─────────────────────────────────────────────────────
        // Get all tasks assigned to a specific student
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TeamTasks>> GetTasksByStudentIdAsync(string studentId)
        {
            return await _context.Tasks
                .Include(t => t.Team)
                .Include(t => t.Assignments)        
                    .ThenInclude(a => a.Student)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Where(t => t.Assignments.Any(a=>a.StudentId == studentId))
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        // ─────────────────────────────────────────────────────
        // Get all tasks across all teams of a supervisor
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TeamTasks>> GetTasksBySupervisorIdAsync(string supervisorId)
        {
            return await _context.Tasks
                .Include(t => t.Team)
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.Student)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Where(t => t.Team.SupervisorId == supervisorId)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        // ─────────────────────────────────────────────────────
        // Get all tasks where deadline passed and still not done
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TeamTasks>> GetOverdueTasksAsync()
        {
            return await _context.Tasks
                .Include(t => t.Team)
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.Student)
                .Where(t => t.DueDate < DateTime.UtcNow && t.Status != true)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        // ─────────────────────────────────────────────────────
        // Get tasks for a team filtered by status
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TeamTasks>> GetTasksByTeamAndStatusAsync(int teamId, bool status)
        {
            return await _context.Tasks
                .Include(t => t.Assignments)
                   .ThenInclude(a => a.Student)
                .Where(t => t.TeamId == teamId && t.Status == status)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }
    }
    }
