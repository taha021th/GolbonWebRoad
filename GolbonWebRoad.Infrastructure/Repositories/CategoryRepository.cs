using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly GolbonWebRoadDbContext _context;

        public CategoryRepository(GolbonWebRoadDbContext context)
        {
            _context=context;
        }
        public Category Add(Category category)
        {
            category.CreatedAt=DateTime.UtcNow;
            _context.Categories.Add(category);
            return category;
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id==id);
            if (category==null)
            {
                throw new NotFoundException("دسته بندی برای حذف یافت نشد.");
            }
            _context.Categories.Remove(category);

        }

        public async Task<IEnumerable<Category>> GetAllAsync(bool? joinProducts = false)
        {
            var query = _context.Categories.AsQueryable();
            if (joinProducts==true)
                query=query.Include(p => p.Products);
            return await query.ToListAsync();

        }

        public async Task<Category?> GetByIdAsync(int id, bool? joinProducts = false)
        {
            var query = _context.Categories.AsQueryable();
            if (joinProducts==true)
                query=query.Include(p => p.Products);


            return await query.FirstOrDefaultAsync(p => p.Id==id);


        }

        public Category Update(Category category)
        {
            _context.Categories.Update(category);
            return category;
        }
    }
}
