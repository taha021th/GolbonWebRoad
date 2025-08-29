using Microsoft.AspNetCore.Http;

namespace GolbonWebRoad.Application.Dtos.Products
{
    public class CreateProductRequestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
