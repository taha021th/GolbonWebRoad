using GolbonWebRoad.Application.Dtos.Categories;
using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{
    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Slog الزامی است.")]
        public string Slog { get; set; }
        [Required(ErrorMessage = "نام الزامی است.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "توضیحات الزامی است.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "قیمت الزامی است.")]
        [Range(1, double.MaxValue, ErrorMessage = "مقدار قیمت باید بیشتر از صفر باشد.")]
        public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; }
        [Required(ErrorMessage = "انتخاب دسته بندی الزامی است.")]
        public int CategoryId { get; set; }
        public IEnumerable<CategorySummaryDto>? Categories { get; set; }
    }
}
