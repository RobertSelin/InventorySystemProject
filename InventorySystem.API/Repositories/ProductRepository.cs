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

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPaginatedAsync(string? search, int page, int pageSize)
        {
            var query = _set.Include(p => p.Category).AsNoTracking().AsQueryable();

            // Filter by search keyword across Name or SKU if search is not null or empty
            if(!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
            }

            // Count total matching rows in Sql Server before slicing items into pages
            var totalCount = await query.CountAsync();

            // We make the pages by slicing items with Orderby, Skip and Take
            var items = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);

        }
    }
}
