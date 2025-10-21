using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    /// <summary>
    /// کلاس نگهدارنده آمار فروش روزانه برای انتقال داده.
    /// </summary>
    public class DailySalesStats
    {
        public DateTime Date { get; set; }
        public decimal Sales { get; set; }
        public int OrdersCount { get; set; }
    }

    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        Order Add(Order order); // برای سازگاری با کدهای قبلی
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        void Update(Order order);
        Task<Order?> GetOrderWithDetailsAsync(int id);

        // === متدهای آماری برای داشبورد ===
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalOrdersCountAsync();
        Task<decimal> GetTodayRevenueAsync();
        Task<int> GetTodayOrdersCountAsync();
        Task<int> GetPendingOrdersCountAsync();
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count = 5);
        Task<IEnumerable<DailySalesStats>> GetDailySalesStatsAsync(int days = 7);
    }
}

