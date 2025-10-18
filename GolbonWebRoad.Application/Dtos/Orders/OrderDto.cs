using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Application.Dtos.Orders
{

    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public ICollection<OrderItemSummaryDto> OrderItems { get; set; } = new List<OrderItemSummaryDto>();

    }

}
