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
            return await _context.Orders.Include(o => o.OrderItems).OrderByDescending(o => o.OrderDate).ToListAsync();
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
    }
}
