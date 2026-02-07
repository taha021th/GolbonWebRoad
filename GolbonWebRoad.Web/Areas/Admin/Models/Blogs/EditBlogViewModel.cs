using GolbonWebRoad.Web.Areas.Admin.Models.BlogCategories;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Blogs
{
    public class EditBlogViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? ShortDescription { get; set; }
        public string? MainImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public int ReadTimeMinutes { get; set; }
        public bool IsPublished { get; set; } = false;
        public string? Slog { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MainImageAltText { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? H1Title { get; set; }
        public string? MetaRobots { get; set; }
        public int CategoryId { get; set; }
        public List<BlogCategoryListItemViewModel> BlogCategoryListItemViewModels { get; set; }
    }
}
