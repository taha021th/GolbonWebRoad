using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class FaqRepository : IFaqRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public FaqRepository(GolbonWebRoadDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Faq faq)
        {
            faq.CreatedAt = DateTime.UtcNow;
            faq.UpdatedAt = DateTime.UtcNow;
            await _context.Faqs.AddAsync(faq);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Faqs.FindAsync(id);
            if (entity == null) throw new NotFoundException("سوال یافت نشد");
            _context.Faqs.Remove(entity);
        }

        public async Task<ICollection<Faq>> GetAllAsync(bool onlyActive = false)
        {
            var q = _context.Faqs.Include(f => f.Category).AsQueryable();
            if (onlyActive) q = q.Where(x => x.IsActive);
            return await q.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        }

        public async Task<Faq?> GetByIdAsync(int id)
        {
            return await _context.Faqs.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Faq?> GetBySlogAsync(string slog)
        {
            return await _context.Faqs.FirstOrDefaultAsync(x => x.Slog == slog);
        }

        public void Update(Faq faq)
        {
            faq.UpdatedAt = DateTime.UtcNow;
            _context.Faqs.Update(faq);
        }
    }
}
