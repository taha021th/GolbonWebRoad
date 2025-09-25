using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IProductAttributeValueRepository
    {
        public void Add(ProductAttributeValue model);
        public Task RemoveAsync(int id);
        public void Update(ProductAttributeValue ProductAttributeValue);
        public Task<PagedResult<ProductAttributeValue>> GetAllAsync(int pageNumber, int pageSize);
        public Task<ProductAttributeValue> GetByIdAsync(int id);
    }
}
