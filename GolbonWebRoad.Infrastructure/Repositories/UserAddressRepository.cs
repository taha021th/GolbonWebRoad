using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class UserAddressRepository : IUserAddressRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public UserAddressRepository(GolbonWebRoadDbContext context)
        {
            _context = context;
        }

        public async Task<UserAddress> AddAsync(UserAddress address)
        {
            _context.UserAddresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.UserAddresses.FindAsync(id);
            if (entity != null)
            {
                _context.UserAddresses.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserAddress?> GetByIdAsync(int id)
        {
            return await _context.UserAddresses.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<UserAddress>> GetByUserIdAsync(string userId)
        {
            return await _context.UserAddresses
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public void Update(UserAddress address)
        {
            _context.UserAddresses.Update(address);
        }
    }
}


