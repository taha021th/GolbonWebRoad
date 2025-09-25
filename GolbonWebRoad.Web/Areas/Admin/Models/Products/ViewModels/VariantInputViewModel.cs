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
    }
}
