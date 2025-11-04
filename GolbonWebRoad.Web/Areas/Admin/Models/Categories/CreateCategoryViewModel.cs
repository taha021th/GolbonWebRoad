using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Categories
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "نام الزامی است.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Slog الزامی است.")]
        public string? Slog { get; set; }
        public string Content { get; set; }
        public IFormFile? Image { get; set; }
    }
}