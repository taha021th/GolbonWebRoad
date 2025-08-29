using GolbonWebRoad.Application.Exceptions;
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
        public async Task<ICollection<Product>> GetAllAsync(string? searchTerm = null, int? categoryId = null, string? sortOrder = null, bool? joinCategory = false)
        {
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(p => p.Name.Contains(searchTerm));


            if (categoryId.HasValue && categoryId>0)
                query=query.Where(p => p.CategoryId==categoryId);


            if (sortOrder != null)
                query = sortOrder switch
                {
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "price_asc" => query.OrderBy(p => p.Price),
                    _ => query.OrderBy(p => p.Name) // حالت پیش‌فرض
                };

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
        public Product Add(Product product)
        {
            product.CreatedAt=DateTime.UtcNow;
            _context.Products.Add(product);
            return product;
        }
        public void Update(Product product)
        {
            _context.Products.Update(product);
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product ==null)
            {
                throw new NotFoundException("محصولی برای حذف یافت نشد.");
            }
            _context.Products.Remove(product);
        }
    }
}
