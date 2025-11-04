namespace GolbonWebRoad.Web.Models.Brands
{
    public class BrandSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class BrandDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public List<GolbonWebRoad.Web.Models.Products.ProductViewModel> Products { get; set; } = new();
    }
}
