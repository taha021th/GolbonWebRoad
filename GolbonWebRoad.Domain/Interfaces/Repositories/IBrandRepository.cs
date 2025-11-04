using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IBrandRepository
    {
        Task<IEnumerable<Brand>> GetAllAsync();
        Task<Brand> GetByIdAsync(int id, bool? joinProduct);
        Task AddAsync(Brand brand);
        void Update(Brand brand);
        Task DeleteAsync(int id);
    }
}
