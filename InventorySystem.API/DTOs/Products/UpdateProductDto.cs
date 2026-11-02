using System.ComponentModel.DataAnnotations;

namespace InventorySystem.API.DTOs.Products
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Barcode { get; set; }

        [Range(0.01, 1_000_000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid CategoryId is required")]
        public int CategoryId { get; set; }
    }
}
