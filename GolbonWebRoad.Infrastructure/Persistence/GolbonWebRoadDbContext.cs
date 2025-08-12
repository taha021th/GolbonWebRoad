using GolbonWebRoad.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Persistence
{
    public class GolbonWebRoadDbContext : DbContext
    {
        public GolbonWebRoadDbContext(DbContextOptions<GolbonWebRoadDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>();
            modelBuilder.Entity<IdentityRole>();
        }
    }
}
