using GolbonWebRoad.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Persistence
{
    public class GolbonWebRoadDbContext : IdentityDbContext
    {
        public GolbonWebRoadDbContext(DbContextOptions<GolbonWebRoadDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Log> Logs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.Slog).IsUnique();
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slog).IsUnique();
            }
            );


            modelBuilder.Entity<ProductColor>()
                .HasKey(pc => new { pc.ProductId, pc.ColorId });
            modelBuilder.Entity<ProductColor>()
                .HasOne(pc => pc.Color)
                .WithMany(p => p.ProductColors)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductColor>()
                .HasOne(pc => pc.Color)
                .WithMany(c => c.ProductColors)
                .HasForeignKey(pc => pc.ColorId);
        }
    }
}