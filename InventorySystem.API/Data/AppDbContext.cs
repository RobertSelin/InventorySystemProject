using InventorySystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.API.Data
{
    public class AppDbContext : DbContext // we open the session to communicate with the database
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<InventoryLog> InventoryLogs => Set<InventoryLog>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<ProductSupplier> ProductSuppliers => Set<ProductSupplier>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(e =>
            {
                e.Property(c => c.Name).HasMaxLength(100).IsRequired();
                e.Property(c => c.Description).HasMaxLength(500);
                e.HasIndex(c => c.Name).IsUnique();
            });

            modelBuilder.Entity<Product>(e =>
            {
                e.Property(p => p.Name).HasMaxLength(200).IsRequired();
                e.Property(p => p.SKU).HasMaxLength(50).IsRequired();
                e.Property(p => p.Price).HasPrecision(18, 2);
                e.HasIndex(p => p.SKU).IsUnique();

                e.HasOne(p => p.Category)
                 .WithMany(c => c.Products)
                 .HasForeignKey(p => p.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict); // do not delete a category if it contains products
            });

            modelBuilder.Entity<InventoryLog>(e =>
            {
                e.Property(l => l.Reason).HasMaxLength(300);
                e.HasOne(l => l.Product)
                 .WithMany(l => l.Logs)
                 .HasForeignKey(l => l.ProductId)
                 .OnDelete(DeleteBehavior.Cascade); // When a product is deleted also its logs are deleted too
            });

            modelBuilder.Entity<Supplier>(e =>
            {
                e.Property(s => s.Name).HasMaxLength(200).IsRequired();
                e.Property(s => s.Email).HasMaxLength(200).IsRequired();
                e.ToTable(t => t.HasCheckConstraint(
                    "CK_Supplier_Email_Format",
                    "[Email] LIKE '%_@_%._%' AND [Email] NOT LIKE '% %'"
                ));
            });

            modelBuilder.Entity<ProductSupplier>(e =>
            {
                e.HasKey(ps => new { ps.ProductId, ps.SupplierId });

                e.Property(ps => ps.SupplyPrice).HasPrecision(18, 2);
                e.HasOne(ps => ps.Product)
                 .WithMany(ps => ps.ProductSuppliers)
                 .HasForeignKey(ps => ps.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(ps => ps.Supplier)
                 .WithMany(ps => ps.ProductSuppliers)
                 .HasForeignKey(ps => ps.SupplierId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data used for test
            modelBuilder.Entity<Category>().HasData(
             new Category { Id = 1, Name = "Electronics", Description = "Devices and accessories" },
             new Category { Id = 2, Name = "Office Supplies", Description = "Paper, pens, folders" },
             new Category { Id = 3, Name = "Furniture", Description = "Desks and chairs" });

            modelBuilder.Entity<Product>().HasData(
              new Product
              {
                  Id = 1,
                  Name = "Laptop 15\"",
                  SKU = "ELE-001",
                  Price = 3499.99m,
                  QuantityInStock = 12,
                  CategoryId = 1,
                  CreateDate = new DateTime(2026, 1, 1)
              },
              new Product
              {
                  Id = 2,
                  Name = "Wireless Mouse",
                  SKU = "ELE-002",
                  Price = 79.90m,
                  QuantityInStock = 40,
                  CategoryId = 1,
                  CreateDate = new DateTime(2026, 1, 1)
              },
              new Product
              {
                  Id = 3,
                  Name = "A4 Paper (500)",
                  SKU = "OFF-001",
                  Price = 24.50m,
                  QuantityInStock = 5,
                  CategoryId = 2,
                  CreateDate = new DateTime(2026, 1, 1)
              });
        }
    }
}
