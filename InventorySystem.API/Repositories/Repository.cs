using InventorySystem.API.Data;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.API.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _set;

        public Repository(AppDbContext context)
        {
            _context = context;
            _set = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _set.FindAsync(id);
        }
        
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _set.AsNoTracking().ToListAsync(); // AsNoTracking - disables the tracking of db (EF core does a snapshot of it and it compares to the object) so it is more efficient in allocating memory
        }

        public async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _set.Update(entity); 
        }

        public void Remove(T entity)
        {
            _set.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0; // return only if the save was successfully made (true = 1)
        }
    }
}
