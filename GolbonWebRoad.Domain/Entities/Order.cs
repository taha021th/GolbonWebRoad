using Microsoft.AspNetCore.Identity;

namespace GolbonWebRoad.Domain.Entities
{
    /// <summary>
    /// سفارشی که کاربر داده با اطلاعاتش
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }

}
