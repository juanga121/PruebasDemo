using Microsoft.EntityFrameworkCore;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Infrastructure.Data;
using System.Linq.Expressions;

namespace PruebasDemo.Infrastructure.Repositories
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey>
        where T : class
        where TKey : notnull
    {
        private readonly DataContext _efContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DataContext efContext)
        {
            _efContext = efContext;
            _dbSet = _efContext.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            SetCreationDate(entity);

            await _dbSet.AddAsync(entity);
            await _efContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TKey id)
        {
            var entity = await FindByIdAsync(id);

            if (entity == null)
                return;

            _dbSet.Remove(entity);
            await _efContext.SaveChangesAsync();
        }

        public async Task<T?> FindByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public List<T> GetByFilter(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Where(filter).ToList();
        }

        public IEnumerable<T> GetByFilterOrdered(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderBy,
            bool? isDesc = true)
        {
            return isDesc == false
                ? _dbSet.Where(predicate).OrderBy(orderBy)
                : _dbSet.Where(predicate).OrderByDescending(orderBy);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _efContext.Entry(entity).State = EntityState.Modified;
            await _efContext.SaveChangesAsync();
        }

        private static void SetCreationDate(T entity)
        {
            var prop = typeof(T).GetProperty("FechaCreacion");

            if (prop != null && prop.PropertyType == typeof(DateTime))
            {
                prop.SetValue(entity, DateTime.UtcNow);
            }
        }
    }
}
