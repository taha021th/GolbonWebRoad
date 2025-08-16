using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Products.Queries;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Api.Controllers
{

    public class ProductsController : ApiBaseController
    {


        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetAll(string? searchTerm, int? categoryId, string? sortOrder, bool? joinCategory = false)
        {
            var products = await Mediator.Send(new GetProductsQuery
            {
                SearchTerm=searchTerm,
                CategoryId=categoryId,
                SortOrder=sortOrder,
                JoinCategory=joinCategory

            });
            return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, bool? joinCategory = false)
        {
            var product = await Mediator.Send(new GetProductByIdQuery { Id = id, JoinCategory=joinCategory });
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST: api/Products
        // نکته: این اندپوینت باید امن‌سازی شود و فقط توسط ادمین قابل دسترسی باشد
        [HttpPost]
        // [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Create([FromForm] CreateProductCommand command) // استفاده از FromForm برای دریافت فایل
        {
            var product = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT: api/Products/5
        // نکته: این اندپوینت باید امن‌سازی شود
        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [FromForm] UpdateProductCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            await Mediator.Send(command);
            return NoContent();
        }

        // DELETE: api/Products/5
        // نکته: این اندپوینت باید امن‌سازی شود
        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteProductCommand { Id = id });
            return NoContent();
        }
    }
}