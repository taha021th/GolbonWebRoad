using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync(bool? joinProducts = false);
        Task<Category?> GetByIdAsync(int id, bool? joinProducts = false);
        Category Add(Category category);
        Category Update(Category category);
        Task DeleteAsync(int id);

    }
}
