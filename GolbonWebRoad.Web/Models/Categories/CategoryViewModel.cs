namespace GolbonWebRoad.Web.Models.Categories
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string? Slog { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public List<GolbonWebRoad.Web.Models.Products.ProductViewModel> Products { get; set; } = new();
    }
}
