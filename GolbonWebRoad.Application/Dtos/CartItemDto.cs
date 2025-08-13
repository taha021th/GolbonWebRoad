namespace GolbonWebRoad.Application.Dtos
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // این خط را اضافه کنید
        public ProductDto Product { get; set; }
    }
}
