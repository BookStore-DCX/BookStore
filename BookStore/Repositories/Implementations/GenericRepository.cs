using BookStore.Data;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly BookContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(BookContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.AsNoTracking().ToListAsync();

        public virtual async Task<T?> GetByIdAsync(params object[] id)
            => await _dbSet.FindAsync(id);

        public virtual async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public virtual Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(params object[] id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task<bool> ExistsAsync(params object[] id)
            => await GetByIdAsync(id) != null;
    }

}
