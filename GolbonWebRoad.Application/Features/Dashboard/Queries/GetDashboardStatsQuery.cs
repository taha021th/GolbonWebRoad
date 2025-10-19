using MediatR;

namespace GolbonWebRoad.Application.Features.Dashboard.Queries
{
    /// <summary>
    /// کوئری برای دریافت آمار و اطلاعات داشبورد پنل ادمین
    /// </summary>
    public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
    {
        // این کوئری پارامتر ورودی ندارد چون آمار کلی سیستم را برمی‌گرداند
    }

    /// <summary>
    /// DTO حاوی آمار و اطلاعات داشبورد
    /// </summary>
    public class DashboardStatsDto
    {
        /// <summary>
        /// مجموع درآمد کل
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// تعداد کل سفارشات
        /// </summary>
        public int TotalOrders { get; set; }

        /// <summary>
        /// تعداد کل کاربران
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// تعداد کل محصولات
        /// </summary>
        public int TotalProducts { get; set; }

        /// <summary>
        /// درآمد امروز
        /// </summary>
        public decimal TodayRevenue { get; set; }

        /// <summary>
        /// تعداد سفارشات امروز
        /// </summary>
        public int TodayOrders { get; set; }

        /// <summary>
        /// تعداد سفارشات در انتظار
        /// </summary>
        public int PendingOrders { get; set; }

        /// <summary>
        /// تعداد محصولات با موجودی کم (کمتر از 10)
        /// </summary>
        public int LowStockProducts { get; set; }

        /// <summary>
        /// آخرین سفارشات (حداکثر 5 مورد)
        /// </summary>
        public List<RecentOrderDto> RecentOrders { get; set; } = new List<RecentOrderDto>();

        /// <summary>
        /// آمار فروش ۷ روز اخیر
        /// </summary>
        public List<DailySalesDto> DailySales { get; set; } = new List<DailySalesDto>();
    }

    /// <summary>
    /// DTO برای آخرین سفارشات
    /// </summary>
    public class RecentOrderDto
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
    }

    /// <summary>
    /// DTO برای آمار فروش روزانه
    /// </summary>
    public class DailySalesDto
    {
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime Date { get; set; }

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