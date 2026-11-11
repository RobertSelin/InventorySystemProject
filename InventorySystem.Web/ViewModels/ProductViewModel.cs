namespace InventorySystem.Web.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;   
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? BarCode { get; set; }
    }
}
