using InventorySystem.API.DTOs;
using InventorySystem.API.DTOs.Products;

namespace InventorySystem.API.Services.Product
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<bool> UpdateAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<ProductDto>> GetPaginatedAsync(string? search, int page, int pageSize);
        Task<ProductDto> AdjustStockAsync(int id, StockAdjustmentDto dto);
        Task<IEnumerable<InventoryLogDto>> GetStockHistoryAsync(int productId);
    }
}
