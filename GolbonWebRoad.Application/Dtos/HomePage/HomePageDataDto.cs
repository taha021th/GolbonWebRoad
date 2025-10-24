using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Application.Dtos.HomePage
{
    public class HomePageDataDto
    {
        public ICollection<Product> Products { get; set; }
        public Product ProductIsFeatured { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
