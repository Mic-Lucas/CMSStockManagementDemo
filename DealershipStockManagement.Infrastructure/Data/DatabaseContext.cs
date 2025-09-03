using DealershipStockManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Image = DealershipStockManagement.Domain.Entities.Image;

namespace DealershipStockManagement.Infrastructure.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<StockItem> StockItems => Set<StockItem>();
        public DbSet<StockAccessory> StockAccessories => Set<StockAccessory>();
        public DbSet<Image> Images => Set<Image>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockItem>()
                .HasMany(s => s.Accessories)
                .WithOne(a => a.StockItem)
                .HasForeignKey(a => a.StockItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StockItem>()
                .HasMany(s => s.Images)
                .WithOne(i => i.StockItem)
                .HasForeignKey(i => i.StockItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StockItem>(entity =>
            {
                entity.Property(e => e.CostPrice)
                      .HasColumnType("decimal(18,2)"); // specify precision and scale

                entity.Property(e => e.RetailPrice)
                      .HasColumnType("decimal(18,2)");

                // Seed sample stock item
                entity.HasData(
                    new StockItem
                    {
                        Id = 1,
                        RegNo = "ABC123GP",
                        Make = "Toyota",
                        Model = "Corolla",
                        ModelYear = 2020,
                        KMS = 15000,
                        Colour = "White",
                        VIN = "1HGCM82633A004352",
                        CostPrice = 180000m,
                        RetailPrice = 220000m,
                        DTCreated = new DateTime(2025, 9, 2),
                        DTUpdated = new DateTime(2025, 9, 2)
                    }
                );
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
