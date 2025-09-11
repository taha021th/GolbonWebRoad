using GolbonWebRoad.Application.Dtos.Colors;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{
    public class CreateProductViewModel
    {
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
        [Range(1, int.MaxValue, ErrorMessage = "لطفا یک {0} معتبر انتخاب کنید.")]
        public int CategoryId { get; set; }

        [Display(Name = "برند")]
        [Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        [Range(1, int.MaxValue, ErrorMessage = "لطفا یک {0} معتبر انتخاب کنید.")]
        public int BrandId { get; set; }

        [Display(Name = "تصاویر محصول")]
        public List<IFormFile>? Images { get; set; }

        [Display(Name = "رنگ های موجود محصول")]
        /// <summary>
        /// Used to receive color data from the dynamic form.
        /// </summary>
        public List<ColorInputDto> Colors { get; set; } = new List<ColorInputDto>();

        // --- Properties for Populating Dropdowns ---

        /// <summary>
        /// Holds the list of categories to be displayed in a dropdown.
        /// This is populated in the controller.
        /// </summary>
        public SelectList? CategoryOptions { get; set; }

        /// <summary>
        /// Holds the list of brands to be displayed in a dropdown.
        /// This is populated in the controller.
        /// </summary>
        public SelectList? BrandOptions { get; set; }
    }
}
