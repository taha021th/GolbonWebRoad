using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public BlogRepository(GolbonWebRoadDbContext context)
        {
            _context=context;
        }

        public async Task AddAsync(Blog blog)
        {
            if (blog.IsPublished==true)
                blog.PublishDate=DateTime.UtcNow;
            await _context.Blogs.AddAsync(blog);
        }

        public void Delete(Blog blog)
        {
            _context.Blogs.Remove(blog);
        }

        public async Task<IEnumerable<Blog>> GetAllAsync(bool? joinBlogCategory)
        {
            var query = _context.Blogs.AsQueryable();
            if (joinBlogCategory == true)
                query=query.Include(bc => bc.BlogCategory);

            return await query.ToListAsync();
        }

        public async Task<Blog> GetByIdAsync(int id, bool? joinBlogCategory, bool? joinBlogReview)
        {
            var query = _context.Blogs.AsQueryable();
            if (joinBlogReview==true)
                query=query.Include(br => br.BlogReviews);
            if (joinBlogCategory==true)
                query=query.Include(bc => bc.BlogCategory);

            return await query.AsNoTracking().FirstOrDefaultAsync(i => i.Id==id);
        }

        public void Update(Blog blog)
        {
            _context.Blogs.Update(blog);
        }
    }
}
