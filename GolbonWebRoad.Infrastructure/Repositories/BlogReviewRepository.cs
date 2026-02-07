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

        public async Task<IEnumerable<BlogReview>> GetAllAsync()
        {
            return await _context.BlogReviews.ToListAsync();
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
