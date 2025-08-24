using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Categories.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ApiBaseController
    {
        private readonly IMemoryCache _cache;
        private string _cacheKey;

        public CategoryController(IMemoryCache cache)
        {
            _cache=cache;
            _cacheKey="allCategories";

        }
        [HttpGet]
        public async Task<IActionResult> GetAll(bool? joinProducts = false)
        {
            if (_cache.TryGetValue(_cacheKey, out var cachedCategories))
            {
                return Ok(cachedCategories);
            }

            var categories = await Mediator.Send(new GetCategoriesQuery { JoinProducts=joinProducts });

            var cacheEntryOptions =
                new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(24));
            _cache.Set(_cacheKey, categories, cacheEntryOptions);

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand model)
        {
            var category = await Mediator.Send(model);
            _cache.Remove(_cacheKey);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryCommand model)
        {
            if (id !=model.Id)
            {
                return BadRequest("Id mismatch");
            }
            var category = await Mediator.Send(model);
            _cache.Remove(_cacheKey);
            return NoContent();

        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteCategoryCommand { Id = id });
            _cache.Remove(_cacheKey);
            return NoContent();

        }
    }
}
