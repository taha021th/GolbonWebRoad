using GolbonWebRoad.Web.Models.BlogCategories;

namespace GolbonWebRoad.Web.Models.Blogs
{
    public class BlogSummaryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ShordDescription { get; set; }
        public string? MainImageUrl { get; set; }
        public int ReadTimeMinutes { get; set; }
        public bool IsPublished { get; set; }
        public DateTime PublishDate { get; set; }
        public string? Slog { get; set; }
        public BlogCategorySummaryViewModel BlogCategory { get; set; }
    }
}
