using AutoMapper;
using GolbonWebRoad.Application.Dtos.Orders;
using GolbonWebRoad.Application.Features.Orders.Commands;
using GolbonWebRoad.Application.Features.Orders.Queries;
using GolbonWebRoad.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ApiBaseController
    {
        private readonly IMapper _mapper;

        public OrdersController(IMapper mapper)
        {
            _mapper=mapper;
        }
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);


        /// <summary>
        /// دریافت سفارش هایی که کاربر لاگین کرده ثبت کرده
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUserOrders")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = GetUserId();
            if (userId==null) return Unauthorized();
            var listOrderDto = await Mediator.Send(new GetOrdersByUserIdQuery { UserId=userId });
            return Ok(listOrderDto);
        }
        /// <summary>
        /// دریافت سفارش با ای دی سفارش ثبت شده
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var orderDto = await Mediator.Send(new GetOrderByIdQuery { Id=id });
            if (orderDto==null) return NotFound(new { message = "سفارش یافت نشد." });
            return Ok(orderDto);

        }
        /// <summary>
        /// داخل پنل ادمین خود ادمین باید تمام سفارشات کاربرا رو ببینه
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> GetAll()
        {
            var listOrderDto = await Mediator.Send(new GetAllOrdersQuery());
            if (listOrderDto==null) return NotFound("در حال حاضر سفارشی ثبت نشده");
            return Ok(listOrderDto);

        }
        /// <summary>
        /// ثبت سفارش کاربر
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto request)
        {
            var command = _mapper.Map<CreateOrderCommand>(request);
            command.UserId = GetUserId();

            await Mediator.Send(command);
            return Ok(new { message = "سفارش با موفقیت ثبت شد." });
        }
        /// <summary>
        /// تغییر وضعیت سفارش کاربر 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(nameof(UpdateStatus))]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusRequestDto request)
        {
            var command = _mapper.Map<UpdateOrderStatusCommand>(request);
            await Mediator.Send(command);
            return NoContent();
        }

    }
}
