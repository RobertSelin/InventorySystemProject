using InventorySystem.API.DTOs;
using InventorySystem.API.DTOs.Products;
using InventorySystem.API.Exceptions;
using InventorySystem.API.Mapping;
using InventorySystem.API.Models;
using InventorySystem.API.Repositories;
using Microsoft.JSInterop.Infrastructure;

namespace InventorySystem.API.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, IRepository<Category> categoryRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        // Get the list with all products
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => p.ToDto());
        }

        // Get a product searching by its Id
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdWithDetailAsync(id);

            if(product == null)
            {
                throw new NotFoundException($"Product with ID {id} was not found");
            }

            return product?.ToDto();
        }

        // Create a product
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            // As a business rule, we verify if Category actually exists before creating a product
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);

            if(category == null)
            {
                throw new NotFoundException($"Category with ID {dto.CategoryId} does not exist");
            }

            // We create and save the new product
            var product = dto.ToEntity();
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            // Attach category name for the response DTO
            product.Category = category;
            return product.ToDto();
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return false; // Product not found
            }

            // In case category is changed, we check if the new category chosen exists
            if(product.CategoryId != dto.CategoryId)
            {
                var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
                if (category == null)
                {
                    throw new NotFoundException($"Category with Id {dto.CategoryId} does not exist.");
                }
            }

            dto.UpdateEntity(product);
            _productRepository.Update(product);

            return await _productRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if(product == null)
            {
                return false;
            }

            _productRepository.Remove(product);
            return await _productRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<ProductDto>> GetPaginatedAsync(string? search, int page, int pageSize)
        {
            var (items, totalCount) = await _productRepository.GetPaginatedAsync(search, page, pageSize);

            return new PagedResult<ProductDto>
            {
                Items = items.Select(p => p.ToDto()),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ProductDto> AdjustStockAsync(int id, StockAdjustmentDto dto)
        {
            if(dto.QuantityChange == 0)
            {
                throw new BusinessRuleException("Quantity change cannot be 0");
            }

            var product = await _productRepository.GetByIdWithDetailAsync(id);
            if(product == null)
            {
                throw new NotFoundException($"Product with ID {id} was not found");
            }

            int previousStock = product.QuantityInStock;
            int newStock = previousStock + dto.QuantityChange;

            if(newStock < 0)
            {
                throw new BusinessRuleException($"Insufficient stock. Current stock is {previousStock}, cannot deduct {Math.Abs(dto.QuantityChange)} items.");
            }

            _logger.LogInformation("Adjusting stock for Product ID {ProductId} ({ProductName}). Change: {Change}, Previous:{PreviousStock}, New: { NewStock}", product.Id, product.Name, dto.QuantityChange, previousStock, newStock);

            product.QuantityInStock = newStock;

            var log = new InventoryLog
            {
                ProductId = product.Id,
                QuantityChange = dto.QuantityChange,
                NewQuantity = newStock,
                Reason = dto.Reason,
                Timestamp = DateTime.UtcNow
            };

            product.Logs.Add(log);

            _productRepository.Update(product);

            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Stock adjustment completed successfully for Product ID {ProductId}.", product.Id);

            return product.ToDto();
        }

        public async Task<IEnumerable<InventoryLogDto>> GetStockHistoryAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if(product == null)
            {
                throw new NotFoundException($"Product with ID {productId} was not found.");
            }

            var logs = await _productRepository.GetStockHistoryAsync(productId);
            return logs.Select(l => l.ToDto());
        }

    }
}
