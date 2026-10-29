using InventorySystem.API.Data;
using InventorySystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.API.Repositories
{
    public class ProductRepository : Repository<Product> , IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetByIdWithDetailAsync(int id)
        {
            return await _set
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetLowStockAsync(int threshold)
        {
            return await _set
                .Include(p => p.Category)
                .Where(p => p.QuantityInStock <= threshold)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
