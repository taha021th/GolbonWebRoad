using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
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
        public async Task<ICollection<Product>> GetAllAsync(string? searchTerm = null, int? categoryId = null, string? sortOrder = null, bool? joinCategory = false, bool? joinReviews = false, bool? joinImages = false, bool? joinBrand = false, int count = 0)
        {
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(p => p.Name.Contains(searchTerm));


            if (categoryId.HasValue && categoryId>0)
                query=query.Where(p => p.CategoryId==categoryId);


            //if (sortOrder != null)
            //    query = sortOrder switch
            //    {
            //        "price_desc" => query.OrderByDescending(p => p.Price),
            //        "price_asc" => query.OrderBy(p => p.Price),
            //        _ => query.OrderBy(p => p.Name)
            //    };

            if (count>0)
                query=query.Take(count);

            if (joinCategory==true)
                query= query.Include(c => c.Category);

            if (joinReviews==true)
                query=query.Include(r => r.Reviews);
            if (joinImages==true)
                query=query.Include(i => i.Images);

            if (joinBrand==true)
                query=query.Include(b => b.Brand);

            query=query.Include(p => p.Variants);


            return await query.ToListAsync();
        }
        public async Task<PagedResult<Product>> GetPagedProductsAsync(int pageNumber, int pageSize, string searchTerm = null, int? categoryId = null, int? brandId = null, string sortOrder = null)
        {
            IQueryable<Product> query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images); // Always include necessary related data

            // Filtering
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (brandId.HasValue) // Brand filter added
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.ShortDescription.Contains(searchTerm));
            }

            // Sorting
            //switch (sortOrder)
            //{
            //    case "price_desc":
            //        query = query.OrderByDescending(p => p.Price);
            //        break;
            //    case "price_asc":
            //        query = query.OrderBy(p => p.Price);
            //        break;
            //    default:
            //        query = query.OrderByDescending(p => p.CreatedAt);
            //        break;
            //}

            // Pagination
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .AsNoTracking()
                                   .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<Product?> GetByIdAsync(int id, bool? joinCategory = false, bool? joinReviews = false, bool? joinImages = false, bool? joinBrand = false)
        {
            var query = _context.Products.AsQueryable();

            if (joinCategory==true)
                query= query.Include(c => c.Category);
            if (joinReviews==true)
                query=query.Include(r => r.Reviews);
            if (joinImages==true)
                query=query.Include(i => i.Images);
            if (joinBrand==true)
                query=query.Include(b => b.Brand);

            query=query.Include(p => p.Variants);

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

        public Task<Product> GetProductByIsFeaturedAsync()
        {
            return _context.Products.Include(p => p.Images).Include(p => p.Variants).FirstOrDefaultAsync(p => p.IsFeatured);
        }
    }
}
