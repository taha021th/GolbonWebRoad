using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Dtos.Products;

namespace GolbonWebRoad.Web.Models
{
    public class ProductViewModel
    {
        public IEnumerable<ProductDto> Products { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; }
    }
}
