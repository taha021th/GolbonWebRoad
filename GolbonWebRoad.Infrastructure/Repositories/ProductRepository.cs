using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Infrastructure.Repositories
{
    /// <summary>
    /// پیاده‌سازی کامل ریپازیتوری محصولات.
    /// این کلاس مسئول اجرای کوئری‌های مربوط به محصولات بر روی دیتابیس است.
    /// </summary>
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

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(p => p.CategoryId == categoryId);

            if (sortOrder != null)
                query = sortOrder switch
                {
                    "price_desc" => query.OrderByDescending(p => p.BasePrice),
                    "price_asc" => query.OrderBy(p => p.BasePrice),
                    "name_desc" => query.OrderByDescending(p => p.Name),
                    "name_asc" => query.OrderBy(p => p.Name),
                    "date_desc" => query.OrderByDescending(p => p.CreatedAt),
                    "date_asc" => query.OrderBy(p => p.CreatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt)
                };

            if (count > 0)
                query = query.Take(count);

            if (joinCategory == true)
                query = query.Include(c => c.Category);

            if (joinReviews == true)
                query = query.Include(r => r.Reviews);

            if (joinImages == true)
                query = query.Include(i => i.Images);

            if (joinBrand == true)
                query = query.Include(b => b.Brand);

            query = query.Include(p => p.Variants);

            return await query.ToListAsync();
        }

        public async Task<PagedResult<Product>> GetPagedProductsAsync(int pageNumber, int pageSize, string searchTerm = null, int? categoryId = null, int? brandId = null, string sortOrder = null)
        {
            IQueryable<Product> query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.ShortDescription.Contains(searchTerm));
            }

            switch (sortOrder)
            {
                case "price_desc": query = query.OrderByDescending(p => p.BasePrice); break;
                case "price_asc": query = query.OrderBy(p => p.BasePrice); break;
                case "name_desc": query = query.OrderByDescending(p => p.Name); break;
                case "name_asc": query = query.OrderBy(p => p.Name); break;
                case "date_desc": query = query.OrderByDescending(p => p.CreatedAt); break;
                case "date_asc": query = query.OrderBy(p => p.CreatedAt); break;
                default: query = query.OrderByDescending(p => p.CreatedAt); break;
            }

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

            if (joinCategory == true) query = query.Include(c => c.Category);
            if (joinReviews == true) query = query.Include(r => r.Reviews);
            if (joinImages == true) query = query.Include(i => i.Images);
            if (joinBrand == true) query = query.Include(b => b.Brand);

            query = query
                .Include(p => p.Variants)
                    .ThenInclude(v => v.AttributeValues)
                        .ThenInclude(av => av.Attribute);

            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Product Add(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
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
            if (product == null)
            {
                throw new NotFoundException("محصولی برای حذف یافت نشد.");
            }
            _context.Products.Remove(product);
        }

        public Task<Product> GetProductByIsFeaturedAsync()
        {
            return _context.Products
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.IsFeatured);
        }

        // ==========================================================
        // === پیاده‌سازی متدهای آماری داشبورد ===
        // ==========================================================

        public async Task<int> GetTotalProductsCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<int> GetLowStockProductsCountAsync()
        {
            // محصولاتی را پیدا می‌کنیم که مجموع موجودی تمام واریانت‌های آن‌ها کمتر از ۱۰ باشد
            return await _context.Products
                .Where(p => p.Variants.Any() && p.Variants.Sum(v => v.StockQuantity) < 10)
                .CountAsync();
        }
    }
}

