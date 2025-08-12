using GolbonWebRoad.Application.Dtos;
using GolbonWebRoad.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        private const string CartSessionKey = "Cart";

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Cart
        [HttpGet]
        public IActionResult GetCart()
        {
            var cart = GetCartFromSession();
            return Ok(cart);
        }

        // POST: api/Cart/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var product = await _mediator.Send(new GetProductByIdQuery { Id = request.ProductId });
            if (product == null) return NotFound("محصول یافت نشد.");

            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            if (existingItem == null)
            {
                cart.Add(new CartItemDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = request.Quantity,
                    ImageUrl = product.ImageUrl
                });
            }
            else
            {
                existingItem.Quantity += request.Quantity;
            }

            SaveCartToSession(cart);
            return Ok(cart);
        }

        // متدهای کمکی
        private List<CartItemDto> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<CartItemDto>() : JsonSerializer.Deserialize<List<CartItemDto>>(cartJson);
        }

        private void SaveCartToSession(List<CartItemDto> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        public class AddToCartRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; } = 1;
        }
    }
}
