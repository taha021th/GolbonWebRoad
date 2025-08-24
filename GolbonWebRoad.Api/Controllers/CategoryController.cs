using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Categories.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Api.Controllers
{

    public class CategoryController : ApiBaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(bool? joinProducts = false)
        {
            var categories = await Mediator.Send(new GetCategoriesQuery { JoinProducts=joinProducts });
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
            return NoContent();

        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteCategoryCommand { Id = id });

            return NoContent();

        }
    }
}
