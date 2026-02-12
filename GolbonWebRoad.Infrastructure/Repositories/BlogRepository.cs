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
        public async Task<IEnumerable<Blog>> GetByActiveIsShowHomePage()
        {
            return await _context.Blogs.AsNoTracking().Where(b => b.IsShowHomePage==true).ToListAsync();
        }

        public async Task<IEnumerable<Blog>> GetAllAsync(bool? joinBlogCategory, int take = 0)
        {
            var query = _context.Blogs.AsQueryable();
            if (take > 0)
                query=query.Take(take);

            if (joinBlogCategory == true)
                query=query.Include(bc => bc.BlogCategory);


            return await query.OrderByDescending(b => b.Id).ToListAsync();
        }

        public async Task<Blog> GetByIdAsync(int id, bool? joinBlogCategory, bool? joinBlogReview, bool? filterByStatusBlogReview)
        {
            var query = _context.Blogs.AsQueryable();
            if (joinBlogReview==true)
                query=query.Include(br => br.BlogReviews);
            if (joinBlogCategory==true)
                query=query.Include(bc => bc.BlogCategory);
            if (joinBlogReview==true)
            {
                query=query.Include(br => br.BlogReviews).ThenInclude(u => u.User);
                if (filterByStatusBlogReview==true)
                {
                    query=query.Include(br => br.BlogReviews.Where(br => br.Status==true)).ThenInclude(u => u.User);
                }
            }





            return await query.AsNoTracking().FirstOrDefaultAsync(i => i.Id==id);
        }

        public void Update(Blog blog)
        {
            _context.Blogs.Update(blog);
        }
    }
}
