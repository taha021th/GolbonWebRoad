using GolbonWebRoad.Application.Dtos.CartItems;

namespace GolbonWebRoad.Application.Dtos.Orders
{
    public class CreateOrderRequestDto
    {
        public List<CartItemSummaryDto> CartItems { get; set; }
    }
}
