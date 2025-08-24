using GolbonWebRoad.Application.Features.Orders.Commands;
using GolbonWebRoad.Application.Features.Orders.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ApiBaseController
    {
        [HttpGet("userId")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId==null) return Unauthorized();
            var listOrderDto = await Mediator.Send(new GetOrdersByUserIdQuery { UserId=userId });
            return Ok(listOrderDto);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var orderDto = await Mediator.Send(new GetOrderByIdQuery { Id=id });
            return Ok(orderDto);

        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var listOrderDto = await Mediator.Send(new GetAllOrdersQuery());
            return Ok(listOrderDto);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId=userId;
            await Mediator.Send(model);
            return NoContent();
        }

        [HttpPost(nameof(UpdateStatus))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusCommand model)
        {
            await Mediator.Send(model);
            return NoContent();
        }

    }
}
