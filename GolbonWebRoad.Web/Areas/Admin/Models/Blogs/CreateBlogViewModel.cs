using GolbonWebRoad.Web.Areas.Admin.Models.BlogCategories;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Blogs
{
    public class CreateBlogViewModel
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; } // خلاصه مطلب
        public string Content { get; set; } // متن اصلی
        public string? MainImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsPublished { get; set; } // وضعیت انتشار
        public string? ShortDescription { get; set; }

        public string? Slog { get; set; }
        public string? H1Title { get; set; }
        public int ReadTimeMinutes { get; set; }
        public string? MetaTitle { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaRobots { get; set; }
        public int CategoryId { get; set; }
        public string? MainImageAltText { get; set; }
        public List<BlogCategoryListItemViewModel> BlogCategoryListItemViewModels { get; set; }

        public DateTime PublishDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
