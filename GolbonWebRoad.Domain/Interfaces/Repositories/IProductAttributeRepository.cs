using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IProductAttributeRepository
    {
        public void Add(ProductAttribute model);
        public Task RemoveAsync(int id);
        public void Update(ProductAttribute model);
        public Task<PagedResult<ProductAttribute>> GetAllAsync(int pageNumber, int pageSize);
        public Task<ProductAttribute> GetByIdAsync(int id);
    }
}