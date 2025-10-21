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

        public async Task<Order> AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            return order;
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
                .Include(o => o.OrderItems).ThenInclude(oi => oi.ProductVariant).ThenInclude(v => v.Product)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.ProductVariant).ThenInclude(v => v.AttributeValues).ThenInclude(av => av.Attribute)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
            return await _context.Orders.Where(o => o.UserId == userId)
                .Include(u => u.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.ProductVariant).ThenInclude(v => v.Product)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.ProductVariant).ThenInclude(v => v.AttributeValues).ThenInclude(av => av.Attribute)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        // === پیاده‌سازی کامل متدهای آماری داشبورد ===

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var validStatus = new[] { "PaymentReceived", "Shipped", "در حال پردازش" };
            return await _context.Orders
                .Where(o => validStatus.Contains(o.OrderStatus))
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.UtcNow.Date;
            var validStatus = new[] { "PaymentReceived", "Shipped", "در حال پردازش" };
            return await _context.Orders
                .Where(o => o.OrderDate.Date == today && validStatus.Contains(o.OrderStatus))
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetTodayOrdersCountAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Orders.CountAsync(o => o.OrderDate.Date == today);
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
            var pendingStatus = new[] { "Pending", "در حال پردازش" };
            return await _context.Orders.CountAsync(o => pendingStatus.Contains(o.OrderStatus));
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
            // برای سازگاری با دیتابیس، از تاریخ UTC استفاده می‌کنیم
            var endDate = DateTime.UtcNow.Date.AddDays(1);
            var startDate = endDate.AddDays(-days);

            // ۱. با یک کوئری بهینه، تمام عملیات گروه‌بندی و جمع‌بندی را به دیتابیس می‌سپاریم.
            var statsDictionary = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate) // استفاده از بازه زمانی به جای o.OrderDate.Date برای سرعت بیشتر
                .GroupBy(o => o.OrderDate.Date) // به دیتابیس می‌گوییم که نتایج را بر اساس روز گروه‌بندی کند
                .Select(g => new DailySalesStats
                {
                    Date = g.Key,
                    // محاسبه مبلغ فقط برای سفارشات معتبر
                    Sales = g.Where(o => o.OrderStatus == "PaymentReceived" || o.OrderStatus == "Shipped" || o.OrderStatus == "در حال پردازش")
                             .Sum(o => o.TotalAmount),
                    OrdersCount = g.Count()
                })
                .ToDictionaryAsync(s => s.Date); // نتیجه را برای دسترسی سریع، در یک دیکشنری می‌ریزیم

            // ۲. یک لیست کامل برای بازه زمانی مورد نظر ایجاد کرده و روزهای بدون فروش را با صفر پر می‌کنیم.
            var result = new List<DailySalesStats>();
            for (int i = 0; i < days; i++)
            {
                var date = DateTime.Today.AddDays(-days + 1 + i);
                if (statsDictionary.TryGetValue(date, out var dayStat))
                {
                    result.Add(dayStat);
                }
                else
                {
                    result.Add(new DailySalesStats { Date = date, Sales = 0, OrdersCount = 0 });
                }
            }

            return result.OrderBy(s => s.Date);
        }
    }
}

