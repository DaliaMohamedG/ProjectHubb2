using System.Linq.Expressions;
using DomainLayer.Contracts;
using Microsoft.EntityFrameworkCore;

namespace PersistenceLayer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<T?> GetByIdAsync(object id)
            => await _context.Set<T>().FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public void Update(T entity) => _context.Set<T>().Update(entity);
        public void Delete(T entity) => _context.Set<T>().Remove(entity);
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }
        public async Task<T> GetEntityWithSpec(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return await query.FirstOrDefaultAsync(predicate);
        }
        public async Task<IEnumerable<T>> ListWithSpec(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return await query.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> ListWithSpec(Expression<Func<T, bool>> predicate, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(predicate).ToListAsync();
        }
    }
}
