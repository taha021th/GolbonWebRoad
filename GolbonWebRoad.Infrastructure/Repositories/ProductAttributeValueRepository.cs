using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class ProductAttributeValueRepository : IProductAttributeValueRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public ProductAttributeValueRepository(GolbonWebRoadDbContext context)
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

        public async Task<IEnumerable<ProductAttributeValue>> GetAllAsync()
        {
            return await _context.ProductAttributeValues.AsNoTracking().ToListAsync();

        }
        public async Task<PagedResult<ProductAttributeValue>> GetAllByPagedAsync(int pageNumber, int pageSize)
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

        public async Task<PagedResult<ProductAttributeValue>> GetAllByAttributeIdAsync(int attributeId, int pageNumber, int pageSize)
        {
            var query = _context.ProductAttributeValues
                .Where(p => p.AttributeId == attributeId)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
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
