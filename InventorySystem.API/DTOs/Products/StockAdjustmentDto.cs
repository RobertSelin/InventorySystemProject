using System.ComponentModel.DataAnnotations;

namespace InventorySystem.API.DTOs.Products
{
    public class StockAdjustmentDto
    {
        /// <summary>
        /// Positive value to add stock (ex: +10 shipment received),
        /// Negative value to deduct stock (ex: -3 sold or damaged).
        /// Cannot be 0 or negative
        /// </summary>

        [Required]
        public int QuantityChange { get; set; }

        [Required(ErrorMessage = "A reason for adjusting stock is required for auditing purposes.")]
        [StringLength(250, ErrorMessage = "Reason cannot exceed 250 characters.")]
        public string Reason { get; set; } = string.Empty;
    }
}
