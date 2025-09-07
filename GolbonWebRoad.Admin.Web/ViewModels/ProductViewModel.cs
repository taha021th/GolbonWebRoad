using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Admin.Web.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام محصول الزامی است")]
        [Display(Name = "نام محصول")]
        public string Name { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Required(ErrorMessage = "قیمت الزامی است")]
        [Range(1, double.MaxValue, ErrorMessage = "قیمت باید معتبر باشد")]
        [Display(Name = "قیمت")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "تعداد موجودی الزامی است")]
        [Range(0, int.MaxValue, ErrorMessage = "موجودی نمی‌تواند منفی باشد")]
        [Display(Name = "تعداد موجودی")]
        public int Stock { get; set; }

        [Display(Name = "دسته‌بندی")]
        public int CategoryId { get; set; }

        [Display(Name = "تصویر محصول")]
        public IFormFile? ImageFile { get; set; } // برای آپلود فایل جدید

        // برای نمایش تصویر فعلی در فرم ویرایش
        public string? ExistingImageUrl { get; set; }

        // برای پر کردن دراپ‌داون دسته‌بندی‌ها
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}