using GolbonWebRoad.Application.Dtos.Products;

namespace GolbonWebRoad.Application.Dtos.CartItems
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int? ColorId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // این خط را اضافه کنید
        public ProductDto Product { get; set; }
    }
}
