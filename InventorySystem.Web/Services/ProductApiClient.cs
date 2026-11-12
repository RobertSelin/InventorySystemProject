using InventorySystem.Web.ViewModels;
using Microsoft.Extensions.Logging.Console;

namespace InventorySystem.Web.Services
{
    public interface IProductApiClientService
    {
        Task<PagedResult<ProductViewModel>?> GetProductAsync(int page = 1, int pageSize = 10, string? search = null);
        Task<ProductViewModel?> GetProductByIdAsync(int id);
        Task<HttpResponseMessage> CreateProductAsync(CreateProductVIewModel model);
        Task<HttpResponseMessage> UpdateProductAsync(int id, UpdateProductViewModel model);
        Task<HttpResponseMessage> DeleteProductAsync(int id);
        Task<HttpResponseMessage> AdjustStockAsync(int id, int quantityChange, string reason);
        Task<HttpResponseMessage> AdjustStockAsync(StockAdjustmentViewModel model);
        Task<IEnumerable<InventoryLogViewModel>?> GetProductHistoryAsync(int id);
        Task<IEnumerable<ProductViewModel>?> GetLowStockProductAsync(int threshold = 10);
    }

    public class ProductApiClient : IProductApiClientService
    {
        private readonly HttpClient _httpClient;

        public ProductApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResult<ProductViewModel>?> GetProductAsync(int page = 1, int pageSize = 10, string? search = null)
        {
            var url = $"/api/products?page={page}&pageSize={pageSize}";
            if(search != null)
            {
                url += $"&search={Uri.EscapeDataString(search)}";
            }

            return await _httpClient.GetFromJsonAsync<PagedResult<ProductViewModel>>(url);
        }

        public async Task<ProductViewModel?> GetProductByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ProductViewModel>($"/api/products/{id}");
        }

        public async Task<HttpResponseMessage> CreateProductAsync(CreateProductVIewModel model)
        {
            return await _httpClient.PostAsJsonAsync("/api/products", model);
        }

        public async Task<HttpResponseMessage> UpdateProductAsync(int id, UpdateProductViewModel model)
        {
            return await _httpClient.PutAsJsonAsync($"/api/products/{id}", model);
        }

        public async Task<HttpResponseMessage> DeleteProductAsync(int id)
        {
            return await _httpClient.DeleteAsync($"/api/products/{id}");
        }

        public async Task<HttpResponseMessage> AdjustStockAsync(int id, int quantityChange, string reason)
        {
            var payload = new { quantityChange, reason };

            return await _httpClient.PostAsJsonAsync($"/api/products/{id}/stock", payload);
        }

        public async Task<HttpResponseMessage> AdjustStockAsync(StockAdjustmentViewModel model)
        {
            var payload = new { model.QuantityChange, model.Reason };

            return await _httpClient.PostAsJsonAsync($"/api/products/{model.ProductId}/stock", payload);
        }

        public async Task<IEnumerable<InventoryLogViewModel>?> GetProductHistoryAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<InventoryLogViewModel>>($"/api/products/{id}/history");
        }

        public async Task<IEnumerable<ProductViewModel>?> GetLowStockProductAsync(int threshold = 10)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProductViewModel>>($"/api/products/low-stock?threshold={threshold}");
        }
    }
}
