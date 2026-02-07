using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.BlogCategories
{
    public class BlogCategoryFromViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام دسته الزامی است")]
        [MaxLength(150)]
        public string? Name { get; set; }

        public string? Slog { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public string? AltTextImageUrl { get; set; }

        // SEO Fields
        [MaxLength(100)]
        public string? MetaTitle { get; set; }
        [MaxLength(200)]
        public string? MetaDescription { get; set; }
        [MaxLength(50)]
        public string? MetaRobots { get; set; }
        public string? CanonicalUrl { get; set; }
    }
}
