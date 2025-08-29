using AutoMapper;
using GolbonWebRoad.Api.CacheRevalidations;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ApiBaseController
    {
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;


        public CategoryController(IMemoryCache cache, IMapper mapper)
        {
            _cache=cache;
            _mapper=mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(bool? joinProducts = false)
        {
            string cacheKey = $"categories-{joinProducts}";
            if (_cache.TryGetValue(cacheKey, out var cachedCategories))
            {
                return Ok(cachedCategories);
            }

            var categories = await Mediator.Send(new GetCategoriesQuery { JoinProducts=joinProducts });

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(24));
            cacheEntryOptions.AddExpirationToken(new CancellationChangeToken(CacheRevalidation.ProductTokenSource.Token));

            _cache.Set(cacheKey, categories, cacheEntryOptions);

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, bool? joinProducts = false)
        {

            var category = await Mediator.Send(new GetCategoryByIdQuery { Id=id, JoinProducts=joinProducts });
            if (category==null) return NotFound();
            return Ok(category);
        }


        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequestDto request)
        {
            var command = _mapper.Map<CreateCategoryCommand>(request);
            var category = await Mediator.Send(command);
            CacheRevalidation.RevalidateProductAndCategoryCache();
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequestDto request)
        {
            if (id !=request.Id)
            {
                return BadRequest("دسته بندی یافت نشد");
            }
            var command = _mapper.Map<UpdateCategoryCommand>(request);
            var category = await Mediator.Send(command);
            CacheRevalidation.RevalidateProductAndCategoryCache();
            return NoContent();

        }


        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteCategoryCommand { Id = id });
            CacheRevalidation.RevalidateProductAndCategoryCache();
            return NoContent();

        }
    }
}
