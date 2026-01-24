using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{
    public class EditProductViewModel
    {
        public int Id { get; set; }

        [Display(Name = "اسلاگ (آدرس URL)")]
        public string? Slog { get; set; }

        [Display(Name = "نام محصول")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string Name { get; set; }

        [Display(Name = "توضیح کوتاه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string ShortDescription { get; set; }

        [Display(Name = "توضیحات کامل")]
        public string Description { get; set; }

        [Display(Name = "قیمت پایه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [Range(1, double.MaxValue, ErrorMessage = "مقدار {0} باید بیشتر از صفر باشد.")]
        public decimal BasePrice { get; set; }

        // SEO Fields
        [Display(Name = "H1 Title")]
        [MaxLength(255)]
        public string? H1Title { get; set; }
        [Display(Name = "Meta Robots")]
        [MaxLength(255)]
        public string? MetaRobots { get; set; }
        [Display(Name = "عنوان سئو (Meta Title)")]
        [MaxLength(255)]
        public string? MetaTitle { get; set; }

        [Display(Name = "توضیحات سئو (Meta Description)")]
        [MaxLength(500)]
        public string? MetaDescription { get; set; }

        [Display(Name = "آدرس کانونیکال (Canonical URL)")]
        [MaxLength(1000)]
        public string? CanonicalUrl { get; set; }

        [Display(Name = "متن جایگزین تصویر اصلی (Alt Text)")]
        [MaxLength(255)]
        public string? MainImageAltText { get; set; }

        [Display(Name = "دسته‌بندی")]
        [Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        public int CategoryId { get; set; }

        [Display(Name = "برند")]
        [Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        public int BrandId { get; set; }

        [Display(Name = "تصویر اصلی فعلی")]
        public string? CurrentMainImageUrl { get; set; }

        [Display(Name = "تصویر اصلی جدید")]
        public IFormFile? NewMainImage { get; set; }

        public List<ProductImageViewModel> Images { get; set; } = new List<ProductImageViewModel>();

        [Display(Name = "افزودن تصاویر جدید")]
        public List<IFormFile>? NewImages { get; set; }

        public List<string> ImagesToDelete { get; set; } = new List<string>();

        public SelectList? CategoryOptions { get; set; }
        public SelectList? BrandOptions { get; set; }

        // --- Variants ---
        public List<VariantRowViewModel> Variants { get; set; } = new List<VariantRowViewModel>();
        public List<AttributeGroupOptionViewModel> AttributeGroups { get; set; } = new();

        [Display(Name = "ویژگی های محصول")]
        public List<AttributeSelectionViewModel> AvailableAttributes { get; set; } = new();
    }
}
