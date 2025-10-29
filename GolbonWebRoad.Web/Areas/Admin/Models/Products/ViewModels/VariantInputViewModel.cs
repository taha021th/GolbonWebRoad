using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{
    public class VariantInputViewModel
    {
        [Display(Name = "SKU")]
        [Required]
        [MaxLength(100)]
        public string Sku { get; set; }

        [Display(Name = "قیمت ورینت")]
        [Range(1, double.MaxValue)]
        public decimal Price { get; set; }

        [Display(Name = "قیمت قدیم")]
        public decimal? OldPrice { get; set; }

        [Display(Name = "موجودی")]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Display(Name = "مقادیر ویژگی‌ها")]
        public List<int> AttributeValueIds { get; set; } = new List<int>();

        [Display(Name = "وزن (گرم)")]
        [Range(0, int.MaxValue)]
        public int Weight { get; set; }

        [Display(Name = "طول (سانتیمتر)")]
        [Range(0, double.MaxValue)]
        public decimal Length { get; set; }

        [Display(Name = "عرض (سانتیمتر)")]
        [Range(0, double.MaxValue)]
        public decimal Width { get; set; }

        [Display(Name = "ارتفاع (سانتیمتر)")]
        [Range(0, double.MaxValue)]
        public decimal Height { get; set; }

        [Display(Name = "GTIN")]
        [MaxLength(50)]
        public string? Gtin { get; set; }

        [Display(Name = "MPN")]
        [MaxLength(100)]
        public string? Mpn { get; set; }
    }
}
