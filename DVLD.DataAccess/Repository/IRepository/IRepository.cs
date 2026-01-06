using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        // For projections
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(
            Expression<Func<T, TResult>> selector, bool distinct = true,
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null);
        Task<IEnumerable<T>> Filter(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        void Remove(T entity);

    }
}
