using GolbonWebRoad.Application.Dtos.Categories;

namespace GolbonWebRoad.Application.Dtos.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public CategorySummaryDto Category { get; set; }
    }
}
