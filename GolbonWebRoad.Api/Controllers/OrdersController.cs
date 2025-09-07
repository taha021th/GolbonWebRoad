using AutoMapper;
using GolbonWebRoad.Application.Dtos.Orders;
using GolbonWebRoad.Application.Features.Orders.Commands;
using GolbonWebRoad.Application.Features.Orders.Queries;
using GolbonWebRoad.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMediator _mediator;

        public OrdersController(IMapper mapper, ILogger<OrdersController> logger, IMediator mediator)
        {
            _mapper=mapper;
            _logger=logger;
            _mediator=mediator;
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
            _logger.LogInformation("درخواست برای دریافت لیست سفارشات کاربر {UserId} دریافت شد.", userId);

            if (userId==null)
            {
                _logger.LogWarning("کاربر احراز هویت نشده تلاش برای دسترسی به سفارشات خود داشت.");
                return Unauthorized();

            };
            var listOrderDto = await _mediator.Send(new GetOrdersByUserIdQuery { UserId=userId });
            _logger.LogInformation("تعداد {OrderCount} سفارش برای کاربر {UserId} یافت و پاسخ ارسال شد.", listOrderDto.Count(), userId);
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
            var userId = GetUserId();
            _logger.LogInformation("کاربر {UserId} درخواست دریافت سفارش با شناسه {OrderId} را ارسال کرد.", userId, id);

            var orderDto = await _mediator.Send(new GetOrderByIdQuery { Id=id });
            if (orderDto==null)
            {
                _logger.LogWarning("سفارش با شناسه {OrderId} که توسط کاربر {UserId} درخواست شده بود، یافت نشد.", id, userId);
                return NotFound(new { message = "سفارش یافت نشد." });
            }

            if (orderDto.UserId != userId)
            {
                _logger.LogError("اقدام به دسترسی غیرمجاز! کاربر {AttemptingUserId} تلاش کرد به سفارش {OrderId} متعلق به کاربر {OwnerUserId} دسترسی پیدا کند.",
                    userId, id, orderDto.UserId);
                //اگر کاربر لاگین کرده باشه ولی بازم اجازه دسترسی نداشته باشه 403 رو ریترن میکنیم
                return Forbid();
            }
            _logger.LogInformation("سفارش با شناسه {OrderId} با موفقیت یافت و برای کاربر {UserId} ارسال شد.", id, userId);
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
            var adminId = GetUserId();
            _logger.LogInformation("ادمین {AdminId} درخواست دریافت تمام سفارشات را ارسال کرد.", adminId);

            var listOrderDto = await _mediator.Send(new GetAllOrdersQuery());

            if (!listOrderDto.Any())
            {
                _logger.LogInformation("هیچ سفارشی برای نمایش به ادمین {AdminId} یافت نشد.", adminId);
                return NotFound("در حال حاضر سفارشی ثبت نشده");
            }
            _logger.LogInformation("تعداد {OrderCount} سفارش یافت و برای ادمین {AdminId} ارسال شد.", listOrderDto.Count(), adminId);
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
            var userId = GetUserId();
            _logger.LogInformation("درخواست ایجاد سفارش برای کاربر {UserId} با {ItemCount} آیتم دریافت شد.", userId, request.CartItems.Count);

            var command = _mapper.Map<CreateOrderCommand>(request);
            command.UserId = GetUserId();
            await _mediator.Send(command);
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
            var adminId = GetUserId();
            _logger.LogInformation("ادمین {AdminId} درخواست تغییر وضعیت سفارش {OrderId} به '{OrderStatus}' را ارسال کرد.", adminId, request.OrderId, request.OrderStatus);

            var command = _mapper.Map<UpdateOrderStatusCommand>(request);
            await _mediator.Send(command);
            _logger.LogInformation("وضعیت سفارش {OrderId} با موفقیت توسط ادمین {AdminId} به '{OrderStatus}' تغییر یافت.", request.OrderId, adminId, request.OrderStatus);
            return NoContent();
        }

    }
}
