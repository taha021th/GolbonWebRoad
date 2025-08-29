using GolbonWebRoad.Application.Dtos.Products;

namespace GolbonWebRoad.Application.Dtos.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ProductSummaryDto> Products { get; set; }
    }
}
