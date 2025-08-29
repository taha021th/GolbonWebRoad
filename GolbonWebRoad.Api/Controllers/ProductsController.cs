using AutoMapper;
using GolbonWebRoad.Api.CacheRevalidations;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Products.Queries;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ApiBaseController
    {
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public ProductsController(IMemoryCache cache, IMapper mapper, IFileStorageService fileStorageService)
        {
            _cache=cache;
            _mapper=mapper;
            _fileStorageService=fileStorageService;
        }
        // GET: api/Products
        /// <summary>
        /// دریافت لیست محصولات با قابلیت فیلتر و مرتب سازی
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetProductsQuery query)
        {
            string cacheKey = $"products-{query.SearchTerm}-{query.CategoryId}-{query.SortOrder}-{query.JoinCategory}";
            if (_cache.TryGetValue(cacheKey, out var cachedProducts))
                return Ok(cachedProducts);

            var products = await Mediator.Send(query);

            var productCacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(24));
            productCacheOptions.AddExpirationToken(new CancellationChangeToken(CacheRevalidation.ProductTokenSource.Token));
            _cache.Set(cacheKey, products, productCacheOptions);
            return Ok(products);
        }

        // GET: api/Products/5
        /// <summary>
        /// دریافت یک محصول
        /// </summary>       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool joinCategory = false)
        {

            var product = await Mediator.Send(new GetProductByIdQuery { Id = id, JoinCategory=joinCategory });
            if (product == null)
            {
                return NotFound(new { Message = $"Product with {id} not found." });
            }
            return Ok(product);
        }

        // POST: api/Products
        /// <summary>
        /// ایجاد یک محصول جدید (نیاز به دسترسی ادمین)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromForm] CreateProductRequestDto request) // استفاده از FromForm برای دریافت فایل
        {
            var command = _mapper.Map<CreateProductCommand>(request);
            if (request.ImageFile!=null)
            {
                command.ImageUrl=await _fileStorageService.SaveFileAsync(request.ImageFile, "products");
            }
            var product = await Mediator.Send(command);
            CacheRevalidation.RevalidateProductAndCategoryCache();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT: api/Products/5        
        /// <summary>
        /// ویرایش یک محصول موجود ( نیاز به دسترسی ادمین)
        /// </summary>

        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductRequestDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("Id in URL does not match Id in body.");
            }
            var command = _mapper.Map<UpdateProductCommand>(request);
            if (request.ImageFile!=null)
            {
                command.ImageUrl=await _fileStorageService.SaveFileAsync(request.ImageFile, "products");
            }
            await Mediator.Send(command);
            CacheRevalidation.RevalidateProductAndCategoryCache();
            return NoContent();
        }

        // DELETE: api/Products/5
        /// <summary>
        /// حذف یک محصول نیاز به دسترسی ادمین
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteProductCommand { Id = id });
            CacheRevalidation.RevalidateProductAndCategoryCache();
            return NoContent();
        }
    }
}