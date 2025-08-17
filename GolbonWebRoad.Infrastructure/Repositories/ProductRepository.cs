using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly GolbonWebRoadDbContext _context;
        public ProductRepository(GolbonWebRoadDbContext context)
        {
            _context = context;
        }
        public async Task<Product> AddAsync(Product product)
        {
            product.CreatedAt=DateTime.UtcNow;
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product !=null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ICollection<Product>> GetAllAsync(string? searchTerm = null, int? categoryId = null, string? sortOrder = null, bool? joinCategory = false)
        {
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));

            }

            if (categoryId.HasValue && categoryId>0)
            {
                query=query.Where(p => p.CategoryId==categoryId);
            }
            if (sortOrder != null)
            {
                query = sortOrder switch
                {
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "price_asc" => query.OrderBy(p => p.Price),
                    _ => query.OrderBy(p => p.Name) // حالت پیش‌فرض
                };
            }

            if (joinCategory==true)
                return await query.Include(c => c.Category).ToListAsync();
            return await query.ToListAsync();


        }


        public async Task<Product?> GetByIdAsync(int id, bool? joinCategory = false)
        {
            var query = _context.Products.AsQueryable();
            if (joinCategory==true)
            {
                query= query.Include(c => c.Category);

            }
            return await query.FirstOrDefaultAsync(p => p.Id==id);


        }

        public async Task UpdateAsync(Product product)
        {

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
