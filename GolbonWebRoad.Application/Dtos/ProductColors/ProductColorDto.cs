using GolbonWebRoad.Application.Dtos.Colors;

namespace GolbonWebRoad.Application.Dtos.ProductColors
{
    public class ProductColorDto
    {
        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public ColorDto Color { get; set; }
    }
}
