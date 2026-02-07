using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IBlogCategoryRepository
    {
        Task<IEnumerable<BlogCategory>> GetAllAsync();
        Task<BlogCategory> GetByIdAsync(int ids);
        Task AddAsync(BlogCategory blogCategory);
        void Update(BlogCategory blogCategory);
        void DeleteAsync(BlogCategory blogCategory);
    }
}
