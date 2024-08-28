using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.Entity.Entities;
using System.Linq.Expressions;

namespace MobitekCRMV2.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(CRMDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddAllAsync(List<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
        public void RemoveRange(List<T> values)
        {
            _dbSet.RemoveRange(values);
        }

        public T Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;

            return entity;
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }




        #region Properties
        public IQueryable<T> Table => _dbSet;
        #endregion
    }
}
