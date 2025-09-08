using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class ColorRepository : IColorRepository
    {
        private readonly GolbonWebRoadDbContext _context;

        public ColorRepository(GolbonWebRoadDbContext context)
        {
            _context = context;
        }

        public async Task<Color?> FindByNameAsync(string name)
        {
            return await _context.Colors.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task AddAsync(Color color)
        {
            await _context.Colors.AddAsync(color);
        }
    }
}
