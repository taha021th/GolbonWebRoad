using GolbonWebRoad.Web.Models.Blogs;

namespace GolbonWebRoad.Web.Models.BlogCategories
{
    public class BlogCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Slog { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public string? AltTextImageUrl { get; set; }
        public string? MetaTtitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaRobots { get; set; }
        public string? CanonicalUrl { get; set; }
        public List<BlogSummaryViewModel> Blogs { get; set; }
    }
}
