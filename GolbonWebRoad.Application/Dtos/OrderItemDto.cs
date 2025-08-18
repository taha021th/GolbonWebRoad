using GolbonWebRoad.Application.Dtos.Products;

namespace GolbonWebRoad.Application.Dtos
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        public OrderDto Order { get; set; }
        public int ProductId { get; set; }
        public ProductDto Product { get; set; }
    }
}
