namespace PersistenceLayer.Repositories
{
    public class TaskRepository(ApplicationDbContext _context) //: ITaskRepository
    {
        //public async Task<IEnumerable<TeamTasks>> GetAllAsync()
        //     => await _context.Tasks
        //    .Include(t => t.Student)
        //    .ToListAsync();

        //public async Task<TeamTasks?> GetByIdAsync(int id)
        //    => await _context.Tasks.FindAsync(id);

        //public Task<IEnumerable<TeamTasks>> GetTasksByProjectAsync(int projectId)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<IEnumerable<TeamTasks>> GetTasksByStudentAsync(int studentId)
        //    => await _context.Tasks
        //    .Where(t => t.StudentId == studentId)
        //    .ToListAsync();
        //public async Task AddAsync(TeamTasks task)
        //    => await _context.AddAsync(task);

        //public void Delete(TeamTasks task)
        //    => _context.Remove(task);
        //public async TeamTasks SaveAsync()
        //    => await _context.SaveChangesAsync();

        //public void UpdateStatus(TeamTasks task)
        //    => _context.Tasks.Update(task);

        //public async Task<IEnumerable<TeamTasks>> GetTasksByDoctorAsync(int doctorId)
        //    => await _context.Tasks
        //    .Where(t => t.DoctorID == doctorId)
        //    .ToListAsync();
    }
}
