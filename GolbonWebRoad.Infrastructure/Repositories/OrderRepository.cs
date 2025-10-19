using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public OrderRepository(GolbonWebRoadDbContext context)
        {
            _context = context;
        }

        public Order Add(Order order)
        {
            _context.Orders.Add(order);
            return order;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.Include(u => u.User).Include(a => a.Address).Include(o => o.OrderItems).OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(v => v.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(v => v.AttributeValues)
                            .ThenInclude(av => av.Attribute)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        // =================================================================================
        // === پیاده‌سازی متد جدید برای خواندن سفارش به همراه جزئیات کامل ===
        // =================================================================================
        public async Task<Order?> GetOrderWithDetailsAsync(int id)
        {
            // این متد برای اطمینان از بارگذاری تمام اطلاعات لازم برای ثبت مرسوله (آیتم‌ها و آدرس) است.
            return await _context.Orders
                .Include(o => o.OrderItems) // آیتم‌های سفارش را بارگذاری کن
                .Include(o => o.Address)    // آدرس سفارش را بارگذاری کن
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
            return await _context.Orders.Where(o => o.UserId == userId)
                .Include(u => u.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(v => v.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(v => v.AttributeValues)
                            .ThenInclude(av => av.Attribute)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        // ==========================================================
        // === پیاده‌سازی متدهای آماری داشبورد ===
        // ==========================================================

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.OrderStatus == "PaymentReceived" || o.OrderStatus == "Shipped")
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.Today;
            return await _context.Orders
                .Where(o => o.OrderDate.Date == today && 
                           (o.OrderStatus == "PaymentReceived" || o.OrderStatus == "Shipped"))
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetTodayOrdersCountAsync()
        {
            var today = DateTime.Today;
            return await _context.Orders
                .Where(o => o.OrderDate.Date == today)
                .CountAsync();
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
            return await _context.Orders
                .Where(o => o.OrderStatus == "Pending")
                .CountAsync();
        }

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count = 5)
        {
            return await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<DailySalesStats>> GetDailySalesStatsAsync(int days = 7)
        {
            var today = DateTime.Today;
            var startDate = today.AddDays(-days + 1);

            var dailyStats = new List<DailySalesStats>();
            
            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                
                var dayRevenue = await _context.Orders
                    .Where(o => o.OrderDate.Date == date && 
                               (o.OrderStatus == "PaymentReceived" || o.OrderStatus == "Shipped"))
                    .SumAsync(o => o.TotalAmount);

                var dayOrdersCount = await _context.Orders
                    .Where(o => o.OrderDate.Date == date)
                    .CountAsync();

                dailyStats.Add(new DailySalesStats
                {
                    Date = date,
                    Sales = dayRevenue,
                    OrdersCount = dayOrdersCount
                });
            }

            return dailyStats;
        }
    }
}
