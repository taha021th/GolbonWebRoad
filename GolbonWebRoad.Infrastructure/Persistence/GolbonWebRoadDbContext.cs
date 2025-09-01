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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Log>().ToTable("Logs");

        }
    }
}
