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
            _context= context;
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
            return await _context.Orders.Include(o => o.User).Include(o => o.OrderItems).ThenInclude(oi => oi.ProductVariant).FirstOrDefaultAsync(o => o.Id==id);
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
            return await _context.Orders.Where(o => o.UserId==userId)
                .Include(u => u.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }
    }
}
