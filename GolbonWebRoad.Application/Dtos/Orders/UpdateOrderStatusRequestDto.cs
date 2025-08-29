namespace GolbonWebRoad.Application.Dtos.Orders
{
    public class UpdateOrderStatusRequestDto
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
    }
}
