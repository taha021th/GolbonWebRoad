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
    public class CartItemController : ApiBaseController
    {

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public CartItemController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var query = new GetCartQuery { UserId= GetUserId() };
            var cart = await _mediator.Send(query);
            return Ok(cart);
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
        {
            var command = _mapper.Map<AddToCartCommand>(request);
            command.UserId=GetUserId();
            await _mediator.Send(command);
            return Ok(new { message = "محصول با موفقیت به سبد خرید اضافه شد." });
        }
        [HttpPut("update/{productId}")]
        public async Task<IActionResult> UpdateCartItem([FromRoute] int productId, [FromBody] UpdateCartItemRequestDto request)
        {
            if (productId!=request.ProductId) return NotFound("محصول مورد نظر یافت نشد");
            var command = _mapper.Map<UpdateCartItemCommand>(request);
            command.UserId=GetUserId();
            await _mediator.Send(command);
            return Ok(new { message = "محصول با موفقیت بروزرسانی شد." });

        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var command = new RemoveCartCommand { ProductId=productId, UserId=GetUserId() };
            await _mediator.Send(command);
            return Ok(new { message = "محصول با موفقیت از سبد خرید حذف شد." });
        }
        [HttpPost("removeAll")]
        public async Task<IActionResult> RemoveAllCartItems()
        {
            var command = new RemoveAllCartItemsCommand() { UserId=GetUserId() };
            await _mediator.Send(command);
            return Ok(new { message = "محصولات شما از سبد خرید حذف شد." });

        }
    }
}
