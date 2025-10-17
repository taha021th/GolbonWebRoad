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

        // اطلاعات آدرس
        [Display(Name = "نام کامل گیرنده")]
        public string AddressFullName { get; set; }

        [Display(Name = "شماره تماس گیرنده")]
        public string AddressPhone { get; set; }

        [Display(Name = "آدرس پستی")]
        public string AddressLine { get; set; }

        [Display(Name = "شهر")]
        public string AddressCity { get; set; }

        [Display(Name = "استان")]
        public string AddressProvince { get; set; }

        [Display(Name = "کد پستی")]
        public string AddressPostalCode { get; set; }

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

