using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace GolbonWebRoad.Application.Features.Products.Queries
{
    public class GetProductsPageDataQuery : IRequest<ProductsPageDataDto>
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string? SortOrder { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 9;
    }

    public class GetProductsPageDataQueryHandler : IRequestHandler<GetProductsPageDataQuery, ProductsPageDataDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IMemoryCache _cache;

        public GetProductsPageDataQueryHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IBrandRepository brandRepository,
            IMemoryCache cache)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _cache = cache;
        }

        public async Task<ProductsPageDataDto> Handle(GetProductsPageDataQuery request, CancellationToken cancellationToken)
        {
            // 1) Try cache for categories/brands (stable data)
            var categoriesKey = "products:categories:all";
            var brandsKey = "products:brands:all";

            if (!_cache.TryGetValue(categoriesKey, out IEnumerable<Category> categories))
            {
                categories = await _categoryRepository.GetAllAsync();
                _cache.Set(categoriesKey, categories, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                    SlidingExpiration = TimeSpan.FromMinutes(10),
                    Priority = CacheItemPriority.Low
                });
            }

            if (!_cache.TryGetValue(brandsKey, out IEnumerable<Brand> brands))
            {
                brands = await _brandRepository.GetAllAsync();
                _cache.Set(brandsKey, brands, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                    SlidingExpiration = TimeSpan.FromMinutes(10),
                    Priority = CacheItemPriority.Low
                });
            }

            // 2) Optionally cache product lists per-filter for a short time
            var productsKey = $"products:list:cat={request.CategoryId?.ToString() ?? "-"}:brand={request.BrandId?.ToString() ?? "-"}:q={request.SearchTerm ?? "-"}:sort={request.SortOrder ?? "-"}:p={request.PageNumber}:ps={request.PageSize}";
            if (!_cache.TryGetValue(productsKey, out PagedResult<Product> pagedProducts))
            {
                pagedProducts = await _productRepository.GetPagedProductsAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.SearchTerm,
                    request.CategoryId,
                    request.BrandId,
                    request.SortOrder);

                _cache.Set(productsKey, pagedProducts, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
                    SlidingExpiration = TimeSpan.FromMinutes(1),
                    Priority = CacheItemPriority.Normal
                });
            }

            return new ProductsPageDataDto
            {
                Products = pagedProducts,
                Categories = categories,
                Brands = brands
            };
        }
    }
}