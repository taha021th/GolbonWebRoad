using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;

namespace GolbonWebRoad.Application.Dtos.Products
{
    public class ProductsPageDataDto
    {
        public PagedResult<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
    }
}