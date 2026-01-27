using DomainLayer.Contracts;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceLayer.Repositories
{
    public class ProjectRepository(ApplicationDbContext _context) : IProjectRepository
    {
        public async Task<IEnumerable<Project>> GetAllAsync()
         => await _context.Projects.ToListAsync();

        public async Task<Project?> GetByIdAsync(int id)
         => await _context.Projects.FindAsync(id);

        public async Task<IEnumerable<Project>> GetProjectsByDoctorAsync(int doctorId)
             => await _context.Projects
            .Where(p => p.DoctorId == doctorId)
            .ToListAsync();

        public async Task<IEnumerable<Project>> GetProjectsByStudentAsync(int studentId)
             => await _context.Projects
            .Where(p => p.Teams.Any(t => t.Students.Any(s => s.Id == studentId)))
            .ToListAsync();

        public async Task<Project?> GetProjectWithDetailsAsync(int id)
            => await _context.Projects
            .Include(p => p.Teams)
                .ThenInclude(t => t.Students)
            .Include(p => p.TeamTasks)
            .Include(p => p.Meetings)
            .Include(p => p.Sponsor)
            .FirstOrDefaultAsync(p => p.Id == id);
        public async Task AddAsync(Project project)
            => await _context.Projects.AddAsync(project);

        public void Delete(Project project)
            => _context.Projects.Remove(project);

        public async Task SaveAsync()
            =>await _context.SaveChangesAsync();

        public void Update(Project project)
            => _context.Projects.Update(project);
    }
}


   
