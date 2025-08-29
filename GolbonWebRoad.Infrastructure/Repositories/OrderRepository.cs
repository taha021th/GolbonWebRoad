using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
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

        public async Task<Order> AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.Include(o => o.OrderItems).OrderByDescending(o => o.OrderDate).ToListAsync();
        }


        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstOrDefaultAsync(o => o.Id==id);
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
            return await _context.Orders.Where(o => o.UserId==userId)
                .Include(u => u.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
