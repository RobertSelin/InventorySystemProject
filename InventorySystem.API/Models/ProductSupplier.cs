namespace InventorySystem.API.Models
{
    public class ProductSupplier
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        public decimal SupplyPrice { get; set; }   
        public int LeadTimeDays { get; set; } // total amount of elapsed time of the process of supplying
    }
}
