namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{
    //public class ProductViewModel
    //{
    //    public int Id { get; set; }
    //    public string Slog { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public decimal Price { get; set; }
    //    public IFormFile? ImageFile { get; set; }
    //    public string? ExistingImageUrl { get; set; }
    //    public int CategoryId { get; set; }
    //    public IEnumerable<CategorySummaryDto>? Categories { get; set; }

    //}

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
