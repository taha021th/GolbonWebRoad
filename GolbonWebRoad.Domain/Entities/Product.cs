using GolbonWebRoad.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolbonWebRoad.Domain.Entities
{
    /// <summary>
    /// محصول پایه (قالب اصلی محصول)
    /// اطلاعات مشترک بین همه متغیرها در اینجا قرار می‌گیرد
    /// </summary>
    public class Product
    {
        public Product()
        {
            // مقداردهی اولیه کلکسیون‌ها برای جلوگیری از خطای NullReferenceException
            Reviews = new HashSet<Review>();
            Variants = new HashSet<ProductVariant>();
            Images = new HashSet<ProductImage>();
        }

        public int Id { get; set; }
        public string? Slog { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }

        // --- روابط ---
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int? BrandId { get; set; }
        [ForeignKey(nameof(BrandId))]
        public virtual Brand Brand { get; set; }

        // کلکسیون متغیرهای این محصول (نام Varient به Variant اصلاح شد)
        public virtual ICollection<ProductVariant> Variants { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        // به جای یک ImageUrl ساده، از یک کلکسیون برای مدیریت تصاویر استفاده می‌کنیم
        public virtual ICollection<ProductImage> Images { get; set; }
    }
    /// <summary>
    /// نوع ویژگی (مثلا: رنگ، سایز، جنس)
    /// </summary>
    public class ProductAttribute
    {
        public ProductAttribute()
        {
            Values = new HashSet<ProductAttributeValue>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        // رابطه یک-به-چند با مقادیر ویژگی
        public virtual ICollection<ProductAttributeValue> Values { get; set; }
    }
    /// <summary>
    /// مقدار یک ویژگی (مثلا: قرمز، لارج، نخی)
    /// </summary>
    public class ProductAttributeValue
    {
        public ProductAttributeValue()
        {
            Variants = new HashSet<ProductVariant>();
        }

        public int Id { get; set; }
        public string Value { get; set; }

        // --- روابط ---
        public int AttributeId { get; set; }
        [ForeignKey(nameof(AttributeId))]
        public virtual ProductAttribute Attribute { get; set; }

        // هر مقدار ویژگی می‌تواند در تعریف چندین متغیر استفاده شود (رابطه چند-به-چند)
        public virtual ICollection<ProductVariant> Variants { get; set; }
    }
}

/// <summary>
/// متغیر محصول (کالای قابل خرید)
/// هر ترکیب از ویژگی‌ها (مثلا رنگ قرمز، سایز لارج) یک متغیر جداگانه است
/// </summary>
public class ProductVariant // نام کلاس از Varient به Variant اصلاح شد
{
    public ProductVariant()
    {
        SelectedAttributes = new HashSet<ProductAttributeValue>();

    }

    public int Id { get; set; }
    public string Sku { get; set; } // شناسه انبارداری منحصر به فرد برای این متغیر
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; } // قیمت قبلی برای نمایش تخفیف
    public int StockQuantity { get; set; } // موجودی انبار فقط برای این متغیر

    // --- روابط ---
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; }

    // ویژگی‌های انتخاب شده برای این متغیر (رابطه چند-به-چند)
    public virtual ICollection<ProductAttributeValue> SelectedAttributes { get; set; }


}
