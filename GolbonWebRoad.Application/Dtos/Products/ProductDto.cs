using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Dtos.ProductColors;
using GolbonWebRoad.Application.Dtos.ProductImages;
using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Application.Dtos.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Slog { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int Quantity { get; set; }
        public string? SKU { get; set; }
        public bool IsFreatured { get; set; }

        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public CategorySummaryDto Category { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ProductImageDto> Images { get; set; }
        public ICollection<ProductColorDto> ProductColors { get; set; }

        public Brand Brand { get; set; }
    }

}
