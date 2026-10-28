namespace InventorySystem.API.Models
{
    public class InventoryLog
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int QuantityChange { get; set; } // +10 received, -3 sold 
        public int NewQuantity { get; set; } // stock after change
        public string? Reason { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
