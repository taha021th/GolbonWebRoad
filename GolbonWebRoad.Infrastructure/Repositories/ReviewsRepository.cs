using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class ReviewsRepository : IReviewsRepository
    {
        private readonly GolbonWebRoadDbContext _context;

        public ReviewsRepository(GolbonWebRoadDbContext context)
        {
            _context=context;
        }
        public void Add(Review review)
        {
            review.ReviewDate = DateTime.UtcNow;
            _context.Reviews.Add(review);

        }

        public async Task DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                throw new NotFoundException("دسته بندی برای حذف یافت نشد.");
            _context.Reviews.Remove(review);
        }

        public async Task<ICollection<Review>> GetAllAsync(bool? joinProducts = false, int take = 0)
        {
            var query = _context.Reviews.AsQueryable();
            if (take>0)
                query=query.Take(take);

            if (joinProducts==true)
                query=query.Include(p => p.Product);
            return await query.ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id, bool? joinProduct = false)
        {
            var query = _context.Reviews.AsQueryable();

            if (joinProduct==true)
                query=query.Include(p => p.Product);

            return await query.FirstOrDefaultAsync(p => p.Id==id);
        }

        public async Task<ICollection<Review>> GetByProductIdAsync(int productId, bool? joinUser = false)
        {
            var query = _context.Reviews
                .Where(r => r.ProductId == productId && r.Status == true) // Only show approved reviews
                .OrderByDescending(r => r.ReviewDate)
                .AsQueryable();

            if (joinUser == true)
                query = query.Include(r => r.User);

            return await query.ToListAsync();
        }

        public async Task<ICollection<Review>> GetReviewsWithDetailsAsync(bool? status = null)
        {
            var query = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                    .ThenInclude(p => p.Images)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            return await query
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public void Update(Review review)
        {
            _context.Reviews.Update(review);
        }
    }
}
