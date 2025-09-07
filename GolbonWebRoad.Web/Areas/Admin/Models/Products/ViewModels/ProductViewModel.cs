using GolbonWebRoad.Application.Dtos.Categories;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Slog { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ExistingImageUrl { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<CategorySummaryDto>? Categories { get; set; }

    }
}
