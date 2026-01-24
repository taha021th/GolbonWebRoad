using GolbonWebRoad.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolbonWebRoad.Domain.Entities
{

    public class Product
    {
        public Product()
        {
            Reviews = new HashSet<Review>();
            Variants = new HashSet<ProductVariant>();
            Images = new HashSet<ProductImage>();
        }

        public int Id { get; set; }
        public string? Slog { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }

        // SEO Fields
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? MainImageAltText { get; set; }
        public string? H1Title { get; set; }
        public string? MetaRobots { get; set; }


        public bool IsFeatured { get; set; }
        public string? MainImageUrl { get; set; } // تصویر اصلی محصول برای نمایش 
        public DateTime CreatedAt { get; set; }

        //Relations
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int? BrandId { get; set; }
        [ForeignKey(nameof(BrandId))]
        public virtual Brand Brand { get; set; }


        public virtual ICollection<ProductVariant> Variants { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<ProductImage> Images { get; set; }
    }
    public class ProductAttribute
    {
        public ProductAttribute()
        {
            Values = new HashSet<ProductAttributeValue>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ProductAttributeValue> Values { get; set; }
    }

    public class ProductAttributeValue
    {
        public ProductAttributeValue()
        {
            Variants = new HashSet<ProductVariant>();
        }

        public int Id { get; set; }
        public string Value { get; set; }


        public int AttributeId { get; set; }
        [ForeignKey(nameof(AttributeId))]
        public virtual ProductAttribute Attribute { get; set; }
        public virtual ICollection<ProductVariant> Variants { get; set; }
    }
}


public class ProductVariant
{
    public ProductVariant()
    {
        AttributeValues = new HashSet<ProductAttributeValue>();

    }

    public int Id { get; set; }
    public string Sku { get; set; } // شناسه انبارداری منحصر به فرد برای این متغیر
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; } // قیمت قبلی برای نمایش تخفیف
    public int StockQuantity { get; set; } // موجودی انبار فقط برای این متغیر

    // SEO - Unique Product Identifiers
    public string? Gtin { get; set; } // Global Trade Item Number (e.g., UPC, EAN)
    public string? Mpn { get; set; } // Manufacturer Part Number
    /// <summary>
    /// وزن گرم
    /// </summary>
    public int Weight { get; set; }
    /// <summary>
    /// طول سانتی متر
    /// </summary>
    public decimal Length { get; set; }
    /// <summary>
    /// عرض سانتی متر
    /// </summary>
    public decimal Width { get; set; }
    /// <summary>
    /// ارتقاع سانتی متر
    /// </summary>
    public decimal Height { get; set; }


    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; }


    public virtual ICollection<ProductAttributeValue> AttributeValues { get; set; }


}
