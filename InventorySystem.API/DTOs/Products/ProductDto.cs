using InventorySystem.API.Models;

namespace InventorySystem.API.DTOs.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Barcode { get; set; }
    }
}
