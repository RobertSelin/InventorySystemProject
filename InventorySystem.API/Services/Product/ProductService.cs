using InventorySystem.API.DTOs.Products;
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

        public ProductService(IProductRepository productRepository, IRepository<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
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
            return product?.ToDto();
        }

        // Create a product
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            // As a business rule, we verify if Category actually exists before creating a product
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);

            if(category == null)
            {
                throw new ArgumentException($"Category with ID {dto.CategoryId} does not exist");
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
                    throw new ArgumentException($"Category with Id {dto.CategoryId} does not exist.");
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
    }
}
