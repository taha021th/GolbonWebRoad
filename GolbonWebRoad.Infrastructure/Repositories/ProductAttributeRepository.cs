using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class ProductAttributeRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public ProductAttributeRepository(GolbonWebRoadDbContext context)
        {
            _context=context;
        }

        public void Add(ProductAttribute model)
        {
            _context.ProductAttributes.Add(model);
        }
        public async Task RemoveAsync(int id)
        {
            var attribute = await _context.ProductAttributes.FindAsync(id);
            _context.ProductAttributes.Remove(attribute);
        }
        public void Update(ProductAttribute model)
        {
            _context.Update(model);
        }

        public async Task<PagedResult<ProductAttribute>> GetAllAsync(int pageNumber, int pageSize)
        {

            var query = _context.ProductAttributes.AsQueryable();


            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .AsNoTracking()
                                   .ToListAsync();

            return new PagedResult<ProductAttribute>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductAttribute> GetByIdAsync(int id)
        {
            return await _context.ProductAttributes.FindAsync(id);
        }

    }
}
