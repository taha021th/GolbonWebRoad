using GolbonWebRoad.Application.Dtos.Categories;

namespace GolbonWebRoad.Web.Areas.Admin.Models
{
    public class CreateProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<CategorySummaryDto>? Categories { get; set; }
    }
}
