using Microsoft.EntityFrameworkCore;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Domain;
using PruebasDemo.Infrastructure.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace PruebasDemo.Infrastructure.Repositories
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey>
        where T : class
        where TKey : notnull
    {
        private readonly DataContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            SetCreationDate(entity);

            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TKey id)
        {
            T? entity = await FindByIdAsync(id);

            if (entity == null)
                return;

            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
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
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        private static void SetCreationDate(T entity)
        {
            PropertyInfo? creationDateProperty = typeof(T).GetProperty(DomainConstants.PropertyCreationDate);

            if (creationDateProperty != null && creationDateProperty.PropertyType == typeof(DateTime))
            {
                creationDateProperty.SetValue(entity, DateTime.UtcNow);
            }
        }
    }
}
