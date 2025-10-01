using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IProductAttributeValueRepository
    {
        public void Add(ProductAttributeValue model);
        public Task RemoveAsync(int id);
        public void Update(ProductAttributeValue ProductAttributeValue);
        Task<IEnumerable<ProductAttributeValue>> GetAllAsync();
        public Task<PagedResult<ProductAttributeValue>> GetAllByPagedAsync(int pageNumber, int pageSize);
        public Task<PagedResult<ProductAttributeValue>> GetAllByAttributeIdAsync(int attributeId, int pageNumber, int pageSize);
        public Task<ProductAttributeValue> GetByIdAsync(int id);
    }
}
