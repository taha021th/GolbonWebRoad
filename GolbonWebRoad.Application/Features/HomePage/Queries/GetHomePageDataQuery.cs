﻿using GolbonWebRoad.Application.Dtos.HomePage;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace GolbonWebRoad.Application.Features.HomePage.Queries
{
    public record GetHomePageDataQuery : IRequest<HomePageDataDto>
    {
    }
    public class GetHomePageDataQueryHandler : IRequestHandler<GetHomePageDataQuery, HomePageDataDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMemoryCache _cache;

        public GetHomePageDataQueryHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IMemoryCache cache)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _cache = cache;
        }
        public async Task<HomePageDataDto> Handle(GetHomePageDataQuery request, CancellationToken cancellationToken)
        {
            // Standard cache-first pattern: try cache, otherwise load from DB then set cache
            var cacheKey = "home:data:v1";

            if (_cache.TryGetValue(cacheKey, out HomePageDataDto cached))
            {
                return cached;
            }

            var products = await _productRepository.GetAllAsync(sortOrder: "price_desc", joinImages: true, joinBrand: true, count: 8);
            var featured = await _productRepository.GetProductByIsFeaturedAsync();
            var categories = await _categoryRepository.GetAllAsync(take: 9);



            var data = new HomePageDataDto
            {
                Products = products,
                ProductIsFeatured = featured,
                Categories = categories
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2),
                Priority = CacheItemPriority.High
            };

            _cache.Set(cacheKey, data, options);

            return data;
        }
    }
}
