using GolbonWebRoad.Application.Features.Orders.Commands;
using GolbonWebRoad.Application.Features.Orders.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ApiBaseController
    {
        [HttpGet("userId")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var listOrderDto = await Mediator.Send(new GetOrdersByUserIdQuery { UserId=userId });
            return Ok(listOrderDto);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var orderDto = await Mediator.Send(new GetOrderByIdQuery { Id=id });
            return Ok(orderDto);

        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var listOrderDto = await Mediator.Send(new GetAllOrdersQuery());
            return Ok(listOrderDto);

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand model)
        {
            await Mediator.Send(model);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost(nameof(UpdateStatus))]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusCommand model)
        {
            await Mediator.Send(model);
            return NoContent();
        }

    }
}
