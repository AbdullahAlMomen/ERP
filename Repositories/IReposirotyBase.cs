using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task UpdateAsync(T entity);
        Task DeleteAsync(object id);
        Task SaveChangesAsync();
    }

    public class RepositoryBase<T>: IRepositoryBase<T> where T:class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public RepositoryBase(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> CreateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
            await SaveChangesAsync();
        }
        public async Task DeleteAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Entity not found");

            _dbSet.Remove(entity);
            await SaveChangesAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
