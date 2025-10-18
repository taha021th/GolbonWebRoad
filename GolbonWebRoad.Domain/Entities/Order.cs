namespace GolbonWebRoad.Domain.Entities
{
    /// <summary>
    /// این کلاس نمایانگر یک سفارش ثبت شده در سیستم است.
    /// تمام اطلاعات اصلی یک سفارش در اینجا نگهداری می‌شود.
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }

        // ==========================================================
        // === فیلدهای اضافه شده برای مدیریت ارسال (لجستیک) ===
        // ==========================================================

        /// <summary>
        /// روش ارسالی که توسط کاربر انتخاب شده است.
        /// مثلا "post-pishtaz" یا "tipax-express"
        /// </summary>
        public string? ShippingMethod { get; set; }

        /// <summary>
        /// هزینه‌ی ارسال که بر اساس روش انتخابی محاسبه شده است.
        /// </summary>
        public decimal ShippingCost { get; set; }

        /// <summary>
        /// شماره رهگیری مرسوله که پس از ثبت در شرکت پستی دریافت می‌شود.
        /// </summary>
        public string? TrackingNumber { get; set; }

        // ==========================================================

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public int? AddressId { get; set; }
        public UserAddress? Address { get; set; }
    }
}

