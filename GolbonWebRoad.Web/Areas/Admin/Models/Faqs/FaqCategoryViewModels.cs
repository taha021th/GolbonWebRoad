using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Faqs
{
    public class FaqCategoryViewModel
    {
        public int Id { get; set; }
        [Required, Display(Name = "نام")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "اسلاگ")]
        public string? Slog { get; set; }
        [Display(Name = "ترتیب")]
        public int SortOrder { get; set; }
        [Display(Name = "فعال")]
        public bool IsActive { get; set; } = true;
    }
}
