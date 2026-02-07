namespace GolbonWebRoad.Web.Models.BlogCategories
{
    public class BlogCategorySummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ShortDescription { get; set; }

        public string? ImageUrl { get; set; }
        public string? AltTextImageUrl { get; set; }

    }
}
