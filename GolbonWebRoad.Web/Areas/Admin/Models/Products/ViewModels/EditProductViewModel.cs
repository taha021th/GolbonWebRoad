using GolbonWebRoad.Application.Dtos.Colors;
using GolbonWebRoad.Application.Dtos.ProductImages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{
    public class EditProductViewModel
    {
        public int Id { get; set; } // <- شناسه محصول برای ویرایش

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

        [Display(Name = "قیمت")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [Range(1, double.MaxValue, ErrorMessage = "مقدار {0} باید بیشتر از صفر باشد.")]
        public decimal Price { get; set; }

        [Display(Name = "قیمت قدیم")]
        [Range(1, double.MaxValue, ErrorMessage = "مقدار {0} باید بیشتر از صفر باشد.")]
        public decimal? OldPrice { get; set; }

        [Display(Name = "موجودی انبار")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار {0} نمی تواند منفی باشد.")]
        public int Quantity { get; set; }

        public string? SKU { get; set; }
        public bool IsFeatured { get; set; }

        [Display(Name = "دسته‌بندی")]
        [Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        public int CategoryId { get; set; }

        [Display(Name = "برند")]
        [Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        public int BrandId { get; set; }

        // --- مدیریت تصاویر ---        
        /// <summary>
        /// تصاویر موجود محصول برای نمایش در فرم
        /// </summary>
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();

        /// <summary>
        /// تصاویر جدیدی که کاربر آپلود می‌کند
        /// </summary>
        [Display(Name = "افزودن تصاویر جدید")]
        public List<IFormFile>? NewImages { get; set; }

        /// <summary>
        /// لیست URL تصاویری که باید حذف شوند
        /// </summary>
        public List<string> ImagesToDelete { get; set; } = new List<string>();

        [Display(Name = "رنگ های فعلی محصول")]
        public List<ExistingColorViewModel> ExistingColors { get; set; } = new();
        [Display(Name = "حذف رنگ ها")]
        public List<int> ColorsToDelete { get; set; } = new();
        // --- مدیریت رنگ‌ها ---
        [Display(Name = "رنگ های موجود محصول")]
        public List<ColorInputDto> NewColors { get; set; } = new List<ColorInputDto>();
        // --- پراپرتی برای دراپ‌داون‌ها ---
        public SelectList? CategoryOptions { get; set; }
        public SelectList? BrandOptions { get; set; }
    }
}