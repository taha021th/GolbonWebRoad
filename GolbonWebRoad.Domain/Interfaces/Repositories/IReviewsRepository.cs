using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IReviewsRepository
    {
        Task<ICollection<Review>> GetAllAsync(bool? joinProducts = false, int take = 0);
        Task<Review?> GetByIdAsync(int id, bool? joinProduct = false);
        Task<ICollection<Review>> GetByProductIdAsync(int productId, bool? joinUser = false);
        Task<ICollection<Review>> GetReviewsWithDetailsAsync(bool? status = null);
        void Add(Review review);
        void Update(Review review);
        Task DeleteAsync(int id);
    }
}
