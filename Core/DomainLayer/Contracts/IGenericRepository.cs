using System.Linq.Expressions;

namespace DomainLayer.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetEntityWithSpec(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> ListWithSpec(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    }
}
