using InventorySystem.API.Models;

namespace InventorySystem.API.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        // because we inherited IRepository interace which has implemented the general queries, here we implement only the doamin specific queries 
        Task<Product?> GetByIdWithDetailAsync(int id);
        Task<IEnumerable<Product>> GetLowStockAsync(int threshold);
        Task<(IEnumerable<Product> Items, int TotalCount)> GetPaginatedAsync(string? search, int page, int pageSize);
        Task<IEnumerable<InventoryLog>> GetStockHistoryAsync(int productId);
    }
}
