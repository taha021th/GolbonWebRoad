using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IBlogRepository
    {
        Task<IEnumerable<Blog>> GetAllAsync(bool? joinBlogCategory);
        Task<Blog> GetByIdAsync(int id, bool? joinBlogCategory, bool? joinBlogReview);
        Task AddAsync(Blog blog);
        void Update(Blog blog);
        void Delete(Blog blog);
    }
}
