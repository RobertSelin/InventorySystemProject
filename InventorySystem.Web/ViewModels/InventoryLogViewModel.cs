namespace InventorySystem.Web.ViewModels
{
    public class InventoryLogViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int QuantityChange { get; set; }
        public int NewQuantity { get; set; }
        public string? Reason { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
