using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    /// <summary>
    /// کلاس نگهدارنده آمار فروش روزانه
    /// </summary>
    public class DailySalesStats
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

    public interface IOrderRepository
    {
        Order Add(Order order);
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        void Update(Order order);
        // ==========================================================
        // === متد جدید برای خواندن سفارش به همراه جزئیات کامل ===
        // ==========================================================
        /// <summary>
        /// یک سفارش را به همراه تمام جزئیات مورد نیاز (آیتم‌ها و آدرس) برای پردازش‌های بعدی برمی‌گرداند.
        /// </summary>
        /// <param name="id">شناسه سفارش</param>
        /// <returns>موجودیت سفارش به همراه جزئیات</returns>
        Task<Order?> GetOrderWithDetailsAsync(int id);

        // ==========================================================
        // === متدهای آماری برای داشبورد ===
        // ==========================================================
        
        /// <summary>
        /// محاسبه مجموع درآمد از سفارشات پرداخت شده و ارسال شده
        /// </summary>
        /// <returns>مجموع درآمد</returns>
        Task<decimal> GetTotalRevenueAsync();

        /// <summary>
        /// تعداد کل سفارشات
        /// </summary>
        /// <returns>تعداد کل سفارشات</returns>
        Task<int> GetTotalOrdersCountAsync();

        /// <summary>
        /// محاسبه درآمد امروز
        /// </summary>
        /// <returns>درآمد امروز</returns>
        Task<decimal> GetTodayRevenueAsync();

        /// <summary>
        /// تعداد سفارشات امروز
        /// </summary>
        /// <returns>تعداد سفارشات امروز</returns>
        Task<int> GetTodayOrdersCountAsync();

        /// <summary>
        /// تعداد سفارشات در انتظار
        /// </summary>
        /// <returns>تعداد سفارشات در انتظار</returns>
        Task<int> GetPendingOrdersCountAsync();

        /// <summary>
        /// دریافت آخرین سفارشات برای داشبورد
        /// </summary>
        /// <param name="count">تعداد سفارشات مورد نیاز</param>
        /// <returns>لیست آخرین سفارشات</returns>
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count = 5);

        /// <summary>
        /// دریافت آمار فروش روزانه برای تعداد روزهای مشخص
        /// </summary>
        /// <param name="days">تعداد روزهای مورد نیاز</param>
        /// <returns>آمار فروش روزانه</returns>
        Task<IEnumerable<DailySalesStats>> GetDailySalesStatsAsync(int days = 7);
    }
}
