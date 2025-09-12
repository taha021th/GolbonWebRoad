using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly GolbonWebRoadDbContext _context;

        public BrandRepository(GolbonWebRoadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }
        public async Task<Brand> GetByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task AddAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
        }
        public void Update(Brand brand)
        {
            _context.Brands.Update(brand);
        }
        public async Task DeleteAsync(int id)
        {
            Brand brand = await _context.Brands.FindAsync(id);
            _context.Brands.Remove(brand);

        }


    }
}
