using GolbonWebRoad.Application.Dtos.Categories;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{

    public class DeleteProductViewModel
    {
        public int Id { get; set; }
        public string Slog { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public CategorySummaryDto Category { get; set; }
    }
}
