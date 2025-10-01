using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public ProductVariantRepository(GolbonWebRoadDbContext context)
        {
            _context=context;
        }
        public void Add(ProductVariant model)
        {
            _context.ProductVariants.Add(model);
        }
        public async Task RemoveAsync(int id)
        {
            var variants = await _context.ProductVariants.FindAsync(id);
            _context.ProductVariants.Remove(variants);
        }
        public void Update(ProductVariant model)
        {
            _context.Update(model);
        }

        public async Task<PagedResult<ProductVariant>> GetAllAsync(int pageNumber, int pageSize)
        {

            var query = _context.ProductVariants.AsQueryable();


            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .AsNoTracking()
                                   .ToListAsync();

            return new PagedResult<ProductVariant>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductVariant> GetByIdAsync(int id)
        {
            return await _context.ProductVariants
                .Include(v => v.Product)
                .Include(v => v.AttributeValues)
                    .ThenInclude(av => av.Attribute)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<ICollection<ProductVariant>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductVariants
                .Include(v => v.AttributeValues)
                .Where(v => v.ProductId == productId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<ProductVariant> GetByIdWithProductAsync(int variantId)
        {
            return await _context.ProductVariants
                .Include(v => v.Product)
                .Include(v => v.AttributeValues)
                    .ThenInclude(av => av.Attribute)
                .FirstOrDefaultAsync(v => v.Id == variantId);
        }
    }
}
