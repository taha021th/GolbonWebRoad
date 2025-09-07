using AutoMapper;
using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Application.Features.CartItems.Commands;
using GolbonWebRoad.Application.Features.CartItems.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartItemController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CartItemController> _logger; // ۲. ILogger را تعریف کنید
        private readonly IMediator _mediator;

        // ۳. ILogger را تزریق کرده و Mediator اضافی را حذف کنید
        public CartItemController(IMapper mapper, ILogger<CartItemController> logger, IMediator mediator)
        {
            _mapper = mapper;
            _logger = logger;
            _mediator=mediator;
        }
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            _logger.LogInformation("کاربر {UserId} درخواست مشاهده سبد خرید خود را ارسال کرد.", userId);

            var query = new GetCartQuery { UserId = userId };
            var cart = await _mediator.Send(query);

            _logger.LogInformation("سبد خرید کاربر {UserId} با {ItemCount} آیتم با موفقیت بازگردانده شد.", userId, cart.Count());
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
        {
            var userId = GetUserId();
            _logger.LogInformation("کاربر {UserId} درخواست افزودن محصول {ProductId} به سبد خرید را ارسال کرد.", userId, request.ProductId);

            var command = _mapper.Map<AddToCartCommand>(request);
            command.UserId = userId;

            await _mediator.Send(command);

            _logger.LogInformation("محصول {ProductId} با موفقیت به سبد خرید کاربر {UserId} اضافه/آپدیت شد.", request.ProductId, userId);
            return Ok(new { message = "محصول با موفقیت به سبد خرید اضافه شد." });
        }

        [HttpPut("update/{productId}")]
        public async Task<IActionResult> UpdateCartItem([FromRoute] int productId, [FromBody] UpdateCartItemRequestDto request)
        {
            var userId = GetUserId();
            _logger.LogInformation("کاربر {UserId} درخواست به‌روزرسانی محصول {ProductId} در سبد خرید به تعداد {Quantity} را ارسال کرد.", userId, productId, request.Quantity);

            // ✅ بهبود مهم: Command باید ProductId را از route بگیرد
            var command = _mapper.Map<UpdateCartItemCommand>(request);
            command.UserId = userId;
            command.ProductId = productId; // ProductId از پارامتر متد خوانده می‌شود

            await _mediator.Send(command);

            _logger.LogInformation("محصول {ProductId} در سبد خرید کاربر {UserId} با موفقیت به‌روزرسانی شد.", productId, userId);
            return Ok(new { message = "محصول با موفقیت بروزرسانی شد." });
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = GetUserId();
            _logger.LogInformation("کاربر {UserId} درخواست حذف محصول {ProductId} از سبد خرید را ارسال کرد.", userId, productId);

            var command = new RemoveCartCommand { ProductId = productId, UserId = userId };
            await _mediator.Send(command);

            _logger.LogInformation("محصول {ProductId} از سبد خرید کاربر {UserId} با موفقیت حذف شد.", productId, userId);
            return Ok(new { message = "محصول با موفقیت از سبد خرید حذف شد." });
        }

        [HttpPost("removeAll")]
        public async Task<IActionResult> RemoveAllCartItems()
        {
            var userId = GetUserId();
            _logger.LogInformation("کاربر {UserId} درخواست حذف تمام آیتم‌ها از سبد خرید خود را ارسال کرد.", userId);

            var command = new RemoveAllCartItemsCommand() { UserId = userId };
            await _mediator.Send(command);

            _logger.LogInformation("تمام آیتم‌ها از سبد خرید کاربر {UserId} با موفقیت حذف شدند.", userId);
            return Ok(new { message = "محصولات شما از سبد خرید حذف شد." });
        }
    }
}