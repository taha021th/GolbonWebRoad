using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class ProductAttributeValueValueRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public ProductAttributeValueValueRepository(GolbonWebRoadDbContext context)
        {
            _context = context;
        }
        public void Add(ProductAttributeValue model)
        {
            _context.ProductAttributeValues.Add(model);
        }
        public async Task RemoveAsync(int id)
        {
            var attributeValue = await _context.ProductAttributeValues.FindAsync(id);
            _context.ProductAttributeValues.Remove(attributeValue);
        }
        public void Update(ProductAttributeValue ProductAttributeValue)
        {
            _context.Update(ProductAttributeValue);
        }

        public async Task<PagedResult<ProductAttributeValue>> GetAllAsync(int pageNumber, int pageSize)
        {

            var query = _context.ProductAttributeValues.AsQueryable();


            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .AsNoTracking()
                                   .ToListAsync();

            return new PagedResult<ProductAttributeValue>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductAttributeValue> GetByIdAsync(int id)
        {
            return await _context.ProductAttributeValues.FindAsync(id);
        }
    }
}
