using InventorySystem.API.DTOs.Products;
using InventorySystem.API.Models;

namespace InventorySystem.API.Mapping
{
    public static class ProductMappingExtensions
    {
        // 1. Entity -> Read DTO (Product -> ProductDto)

        public static ProductDto ToDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Barcode = product.Barcode,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                CreateDate = product.CreateDate,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty
            };
        }

        // Create DTO -> Entity

        public static Product ToEntity(this CreateProductDto dto)
        {
            return new Product
            {
                Name = dto.Name,
                SKU = dto.SKU,
                Barcode = dto.Barcode,
                Price = dto.Price,
                QuantityInStock = dto.InitialQuantity,
                CategoryId = dto.CategoryId,
                CreateDate = DateTime.UtcNow,
            };
        }

        // Update DTO -> existing Entity

        public static void UpdateEntity (this UpdateProductDto dto, Product product)
        {
            product.Name = dto.Name;
            product.Barcode = dto.Barcode;
            product.Price = dto.Price;  
            product.CategoryId = dto.CategoryId;
        }

        public static InventoryLogDto ToDto(this InventoryLog log)
        {
            return new InventoryLogDto
            {
                Id = log.Id,
                ProductId = log.ProductId,
                QuantityChange = log.QuantityChange,
                NewQuantity = log.NewQuantity,
                Reason  = log.Reason,
                TimeStamp = log.Timestamp,
            };
        }
    }
}
