namespace InventorySystem.API.DTOs.Products
{
    public class InventoryLogDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }  
        public int QuantityChange { get; set; }
        public int NewQuantity { get; set; }   
        public string? Reason { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
