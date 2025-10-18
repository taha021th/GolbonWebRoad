using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// ویومدل برای نمایش جزئیات کامل یک سفارش در پنل ادمین.
    /// این نسخه، قابلیت‌های قبلی و جدید را با هم ادغام می‌کند.
    /// </summary>
    public class OrderDetailViewModel
    {
        public int Id { get; set; }

        [Display(Name = "نام کاربری مشتری")]
        public string UserName { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public string OrderDate { get; set; }

        [Display(Name = "وضعیت سفارش")]
        public string OrderStatus { get; set; }

        [Display(Name = "مبلغ نهایی")]
        public decimal TotalAmount { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        [Display(Name = "آدرس تحویل")]
        public UserAddressViewModel UserAddress { get; set; }

        // ==========================================================
        // === پراپرتی‌های اضافه شده برای نمایش اطلاعات ارسال ===
        // ==========================================================
        [Display(Name = "روش ارسال")]
        public string? ShippingMethod { get; set; }

        [Display(Name = "هزینه ارسال")]
        public decimal ShippingCost { get; set; }

        [Display(Name = "کد رهگیری")]
        public string? TrackingNumber { get; set; }
    }
}

