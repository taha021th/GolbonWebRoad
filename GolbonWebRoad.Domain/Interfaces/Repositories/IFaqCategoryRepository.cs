using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IFaqCategoryRepository
    {
        Task<ICollection<FaqCategory>> GetAllAsync(bool onlyActive = false);
        Task<FaqCategory?> GetByIdAsync(int id);
        Task AddAsync(FaqCategory category);
        void Update(FaqCategory category);
        Task DeleteAsync(int id);
    }
}
