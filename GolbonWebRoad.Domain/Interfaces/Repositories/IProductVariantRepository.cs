using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IProductVariantRepository
    {
        void Add(ProductVariant model);
        Task RemoveAsync(int id);
        void Update(ProductVariant model);
        Task<PagedResult<ProductVariant>> GetAllAsync(int pageNumber, int pageSize);
        Task<ProductVariant> GetByIdAsync(int id);
        Task<ICollection<ProductVariant>> GetByProductIdAsync(int productId);
    }
}
