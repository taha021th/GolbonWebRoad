using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetAllAsync(bool? joinProducts = false);
        Task<Category?> GetByIdAsync(int id, bool? joinProducts = false);
        Task<Category> AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task DeleteAsync(int id);

    }
}
