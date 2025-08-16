using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Categories.Queries;
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
        public async Task<IActionResult> Create([FromForm] CreateCategoryCommand command)
        {
            var category = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] UpdateCategoryCommand command)
        {
            if (id!=command.Id)
            {
                return NotFound();
            }
            var category = await Mediator.Send(command);
            return Ok(category);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteCategoryCommand { Id = id });

            return NoContent();

        }
    }
}
