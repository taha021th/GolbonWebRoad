using GolbonWebRoad.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Persistence
{
    public class GolbonWebRoadDbContext : IdentityDbContext<ApplicationUser>
    {
        public GolbonWebRoadDbContext(DbContextOptions<GolbonWebRoadDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<FaqCategory> FaqCategories { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<BlogReview> BlogReviews { get; set; }
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
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasIndex(b => b.Slog).IsUnique();
            });

            // FAQ Category Config
            modelBuilder.Entity<FaqCategory>(entity =>
            {
                entity.HasIndex(c => c.Slog).IsUnique();
                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
            });

            // FAQ Config
            modelBuilder.Entity<Faq>(entity =>
            {
                entity.HasIndex(f => f.Slog).IsUnique();
                entity.Property(f => f.Question).IsRequired();
                entity.Property(f => f.Answer).IsRequired();
                entity.Property(f => f.Tags).HasMaxLength(500);
                entity.HasOne(f => f.Category)
                      .WithMany(c => c.Faqs)
                      .HasForeignKey(f => f.FaqCategoryId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

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

            // Configure Many-to-Many relationship between ProductVariant and ProductAttributeValue
            modelBuilder.Entity<ProductVariant>()
                .HasMany(pv => pv.AttributeValues)
                .WithMany(av => av.Variants)
                .UsingEntity("ProductAttributeValueProductVariant",
                    l => l.HasOne(typeof(ProductAttributeValue)).WithMany().HasForeignKey("AttributeValuesId"),
                    r => r.HasOne(typeof(ProductVariant)).WithMany().HasForeignKey("VariantsId"),
                    j => j.HasKey("AttributeValuesId", "VariantsId"));

            modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.PhoneNumber).IsUnique(true);
        }
    }
}



