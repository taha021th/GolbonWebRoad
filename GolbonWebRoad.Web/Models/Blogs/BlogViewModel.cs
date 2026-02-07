namespace GolbonWebRoad.Web.Models.Blogs
{
    public class BlogViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? ShortDescription { get; set; }
        public string? MainImageUrl { get; set; }
        public int ReadTimeMinutes { get; set; }
        public bool IsPublished { get; set; }
        public DateTime PublishDate { get; set; }
        //seo fields
        public string? Slog { get; set; }
        public string? MetaTtitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MainImageAltText { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? H1Title { get; set; }
        public string? MetaRobots { get; set; }
    }
}
