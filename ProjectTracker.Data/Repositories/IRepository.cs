using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectTracker.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        // Get methods
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params Expression<Func<T, object>>[] includes);

        // Add methods
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        // Update methods
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);

        // Delete methods
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);

        // Other methods
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
        IQueryable<T> GetQueryable();
    }
}