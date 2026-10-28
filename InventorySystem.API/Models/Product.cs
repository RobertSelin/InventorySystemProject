namespace InventorySystem.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public int CategoryId { get; set; } // foreign key
        public Category Category { get; set; } = null!; // navigation property

        public ICollection<InventoryLog> Logs { get; set; } = new List<InventoryLog>();
        public ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
    }
}
