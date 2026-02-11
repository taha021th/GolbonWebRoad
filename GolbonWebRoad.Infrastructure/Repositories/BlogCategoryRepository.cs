using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class BlogCategoryRepository : IBlogCategoryRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public BlogCategoryRepository(GolbonWebRoadDbContext context)
        {
            _context=context;
        }
        public async Task AddAsync(BlogCategory blogCategory)
        {
            await _context.BlogCategories.AddAsync(blogCategory);
        }

        public void DeleteAsync(BlogCategory blogCategory)
        {

            _context.BlogCategories.Remove(blogCategory);
        }

        public async Task<IEnumerable<BlogCategory>> GetAllAsync()
        {
            return await _context.BlogCategories.ToListAsync();
        }

        public async Task<BlogCategory> GetByIdAsync(int id, bool? joinBlogs)
        {
            var query = _context.BlogCategories.AsQueryable();
            if (joinBlogs==true)
                query.Include(b => b.Blogs);
            return await query.AsNoTracking().FirstOrDefaultAsync(bc => bc.Id==id);
        }

        public void Update(BlogCategory blogCategory)
        {
            _context.BlogCategories.Update(blogCategory);
        }
    }
}
