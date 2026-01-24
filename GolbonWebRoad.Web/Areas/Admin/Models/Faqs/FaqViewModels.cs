using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Faqs
{
    public class FaqViewModel
    {
        public int Id { get; set; }
        public string? Slog { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public int? CategoryId { get; set; }
        public string? Tags { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateFaqViewModel
    {
        [Display(Name = "اسلاگ")]
        public string? Slog { get; set; }
        [Required, Display(Name = "سوال")]
        public string Question { get; set; } = string.Empty;
        [Required, Display(Name = "پاسخ (HTML)")]
        public string Answer { get; set; } = string.Empty;
        [Display(Name = "دسته‌بندی")]
        public int? CategoryId { get; set; }
        public SelectList? CategoryOptions { get; set; }
        [Display(Name = "تگ‌ها (جداشده با ,)")]
        public string? Tags { get; set; }
        [Display(Name = "ترتیب")]
        public int SortOrder { get; set; }
        [Display(Name = "فعال")]
        public bool IsActive { get; set; } = true;
    }

    public class EditFaqViewModel : CreateFaqViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
