using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "نام مصحول الزامی است")]
        [Display(Name = "نام محصول")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Required(ErrorMessage = "قیمت محصول الزامی است")]
        [Range(1, double.MaxValue, ErrorMessage = "قیمت باید بیشتر از صفر باشد")]
        [Display(Name = "قیمت")]
        public decimal Price { get; set; }

        [Display(Name = "تصویر محصول")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }
    }
}
