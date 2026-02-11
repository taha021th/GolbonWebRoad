using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class BlogReviewRepository : IBlogReviewRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public BlogReviewRepository(GolbonWebRoadDbContext context)
        {
            _context=context;
        }
        public async Task AddAsync(BlogReview blogReview)
        {
            await _context.BlogReviews.AddAsync(blogReview);
        }

        public async Task DeleteAsync(int id)
        {
            BlogReview blogReview = await _context.BlogReviews.FindAsync(id);
            _context.BlogReviews.Remove(blogReview);
        }

        public async Task<IEnumerable<BlogReview>> GetAllAsync(bool filterByStatus)
        {
            var query = _context.BlogReviews.AsQueryable();
            if (filterByStatus==true)
                query=query.Where(b => b.Status==true);

            return await query.ToListAsync();
        }

        public async Task<ICollection<BlogReview>> GetBlogReviewsWithDetailAsync(bool? status = null)
        {

            var query = _context.BlogReviews
                .Include(r => r.User)
                .Include(r => r.Blogs)
                .AsQueryable();
            if (status.HasValue)
            {
                query=query.Where(r => r.Status==status.Value);
            }
            return await query.OrderByDescending(r => r.ReviewDate).ToListAsync();
        }

        public async Task<BlogReview> GetByIdAsync(int id)
        {
            var query = _context.BlogReviews.AsQueryable();
            return await query.FirstOrDefaultAsync(br => br.Id==id);
        }

        public void Update(BlogReview blogReview)
        {
            _context.BlogReviews.Update(blogReview);
        }
    }
}
