using DVLD.DataAccess.Data;
using DVLD.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            return await GetAllAsync(filter, includeProperties);
        }

        public async Task<T> GetAsync(
                Expression<Func<T, bool>>? filter = null,
                string? includeProperties = null,
                bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if (!tracked)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var IncludeProp in includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(IncludeProp);
                }
            }

            return await query.ToListAsync();
        }

        // For projections
        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(
        Expression<Func<T, TResult>> selector, bool distinct = true,
        Expression<Func<T, bool>>? filter = null,
        string? includeProperties = null)
        {
            if (distinct)
                return await BuildQuery(filter, includeProperties)
                .Select(selector).Distinct()
                .ToListAsync();

            return await BuildQuery(filter, includeProperties)
                .Select(selector)
                .ToListAsync();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.CountAsync();
        }

        #region Private Mehtods

        private IQueryable<T> BuildQuery(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query;
        }

        #endregion
    }
}
