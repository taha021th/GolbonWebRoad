using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Categories
{
    public class EditCategoryViewModel
    {
        [Required(ErrorMessage = "شناسه اجباری است.")]
        public int Id { get; set; }
        [Required(ErrorMessage = "نام الزامی است")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Slog الزامی است.")]
        public string? Slog { get; set; }
    }
}
