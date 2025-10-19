using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Dashboard
{
    /// <summary>
    /// ویو مدل داشبورد پنل ادمین که حاوی آمار و اطلاعات کلیدی سیستم است
    /// </summary>
    public class DashboardViewModel
    {
        /// <summary>
        /// مجموع درآمد کل
        /// </summary>
        [Display(Name = "مجموع فروش")]
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// تعداد کل سفارشات
        /// </summary>
        [Display(Name = "تعداد سفارشات")]
        public int TotalOrders { get; set; }

        /// <summary>
        /// تعداد کل کاربران
        /// </summary>
        [Display(Name = "تعداد کاربران")]
        public int TotalUsers { get; set; }

        /// <summary>
        /// تعداد کل محصولات
        /// </summary>
        [Display(Name = "تعداد محصولات")]
        public int TotalProducts { get; set; }

        /// <summary>
        /// آخرین سفارشات برای نمایش در داشبورد
        /// </summary>
        public List<RecentOrderViewModel> RecentOrders { get; set; } = new List<RecentOrderViewModel>();

        /// <summary>
        /// آمار فروش ۷ روز اخیر برای نمودار
        /// </summary>
        public List<DailySalesViewModel> DailySales { get; set; } = new List<DailySalesViewModel>();

        /// <summary>
        /// درآمد امروز
        /// </summary>
        [Display(Name = "درآمد امروز")]
        public decimal TodayRevenue { get; set; }

        /// <summary>
        /// تعداد سفارشات امروز
        /// </summary>
        [Display(Name = "سفارشات امروز")]
        public int TodayOrders { get; set; }

        /// <summary>
        /// تعداد سفارشات در انتظار
        /// </summary>
        [Display(Name = "سفارشات در انتظار")]
        public int PendingOrders { get; set; }

        /// <summary>
        /// تعداد محصولات با موجودی کم
        /// </summary>
        [Display(Name = "محصولات کم موجود")]
        public int LowStockProducts { get; set; }
    }

    /// <summary>
    /// ویو مدل برای نمایش آخرین سفارشات در داشبورد
    /// </summary>
    public class RecentOrderViewModel
    {
        /// <summary>
        /// شناسه سفارش
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// نام کاربر سفارش دهنده
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// مبلغ سفارش
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// وضعیت سفارش
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// تاریخ ثبت سفارش
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// متن نمایشی زمان سفارش (مثل "۵ دقیقه پیش")
        /// </summary>
        public string TimeAgo { get; set; }
    }

    /// <summary>
    /// ویو مدل برای آمار فروش روزانه جهت نمایش در نمودار
    /// </summary>
    public class DailySalesViewModel
    {
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// نام روز به فارسی
        /// </summary>
        public string DayName { get; set; }

        /// <summary>
        /// مبلغ فروش آن روز
        /// </summary>
        public decimal Sales { get; set; }

        /// <summary>
        /// تعداد سفارشات آن روز
        /// </summary>
        public int OrdersCount { get; set; }
    }
}