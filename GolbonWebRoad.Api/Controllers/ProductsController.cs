using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Products.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ApiBaseController
    {


        // GET: api/Products
        /// <summary>
        /// دریافت لیست محصولات با قابلیت فیلتر و مرتب سازی
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetProductsQuery query)
        {
            var products = await Mediator.Send(new GetProductsQuery());
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CreateProductCommand command) // استفاده از FromForm برای دریافت فایل
        {
            var product = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT: api/Products/5        
        /// <summary>
        /// ویرایش یک محصول موجود ( نیاز به دسترسی ادمین)
        /// </summary>

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Id in URL does not match Id in body.");
            }
            await Mediator.Send(command);
            return NoContent();
        }

        // DELETE: api/Products/5
        /// <summary>
        /// حذف یک محصول نیاز به دسترسی ادمین
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteProductCommand { Id = id });
            return NoContent();
        }
    }
}