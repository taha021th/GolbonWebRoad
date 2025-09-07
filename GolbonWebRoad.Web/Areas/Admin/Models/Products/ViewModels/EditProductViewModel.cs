using GolbonWebRoad.Application.Dtos.Categories;
using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{
    public class EditProductViewModel
    {
        [Required(ErrorMessage = "شناسه محصول الزامی است.")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Slog محصول الزامی است.")]
        public string? Slog { get; set; }
        [Required(ErrorMessage = "نام محصول الزامی است.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "قیمت محصول الزامی است.")]
        [Range(1, double.MaxValue, ErrorMessage = "مقدار قیمت باید بیشتر از صفر باشد.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "توضیحات محصول الزامی است.")]
        public string Description { get; set; }
        [Required(ErrorMessage = " انتخاب دسته بندی محصول الزامی است.")]
        public int CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ExistingImageUrl { get; set; }
        public IEnumerable<CategorySummaryDto>? Categories { get; set; }
    }
}
