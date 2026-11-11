using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Web.ViewModels
{
    public class StockAdjustmentViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }

        [Required(ErrorMessage = "Quantity change is required.")]
        public int QuantityChange { get; set; }

        [Required(ErrorMessage = "A reason is required for auditing purposes.")]
        [StringLength(250, ErrorMessage = "Reason cannot exceed 250 characters.")]
        public string Reason { get; set; } = string.Empty;
    }
}
