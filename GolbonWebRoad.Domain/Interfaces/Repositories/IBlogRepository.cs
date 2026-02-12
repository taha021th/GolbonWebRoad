using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IBlogRepository
    {
        Task<IEnumerable<Blog>> GetByActiveIsShowHomePage();
        Task<IEnumerable<Blog>> GetAllAsync(bool? joinBlogCategory, int take = 0);
        Task<Blog> GetByIdAsync(int id, bool? joinBlogCategory, bool? joinBlogReview, bool? filterByStatusBlogReview);
        Task AddAsync(Blog blog);
        void Update(Blog blog);
        void Delete(Blog blog);
    }
}
