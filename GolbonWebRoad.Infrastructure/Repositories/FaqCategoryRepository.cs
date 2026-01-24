using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class FaqCategoryRepository : IFaqCategoryRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public FaqCategoryRepository(GolbonWebRoadDbContext context) { _context = context; }

        public async Task AddAsync(FaqCategory category)
        {
            await _context.FaqCategories.AddAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.FaqCategories.FindAsync(id);
            if (entity == null) throw new NotFoundException("دسته سوالات یافت نشد");
            _context.FaqCategories.Remove(entity);
        }

        public async Task<ICollection<FaqCategory>> GetAllAsync(bool onlyActive = false)
        {
            var q = _context.FaqCategories.AsQueryable();
            if (onlyActive) q = q.Where(c => c.IsActive);
            return await q.OrderBy(c => c.SortOrder).ThenBy(c => c.Id).ToListAsync();
        }

        public async Task<FaqCategory?> GetByIdAsync(int id)
        {
            return await _context.FaqCategories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public void Update(FaqCategory category)
        {
            _context.FaqCategories.Update(category);
        }
    }
}
