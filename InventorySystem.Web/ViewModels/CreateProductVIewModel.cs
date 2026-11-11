using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Web.ViewModels
{
    public class CreateProductVIewModel
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "SKU must be between 3 and 50 characters.")]
        public string SKU { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Barcode { get; set; }

        [Range(0.01, 1_000_000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Initial quantity cannot be negative.")]
        public int InitialQuantity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Category.")]
        public int CategoryId { get; set; }
    }
}
