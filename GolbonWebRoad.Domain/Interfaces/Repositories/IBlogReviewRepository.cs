using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IBlogReviewRepository
    {
        Task<IEnumerable<BlogReview>> GetAllAsync(bool filterByStatus);
        Task<ICollection<BlogReview>> GetBlogReviewsWithDetailAsync(bool? status = null);
        Task<BlogReview> GetByIdAsync(int id);
        Task AddAsync(BlogReview blogReview);
        void Update(BlogReview blogReview);
        Task DeleteAsync(int id);
    }
}
