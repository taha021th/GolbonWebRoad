using GolbonWebRoad.Application.Dtos;
using GolbonWebRoad.Application.Features.Products.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ApiBaseController
    {

        private const string CartSessionKey = "Cart";

        [HttpGet]

        public IActionResult GetCart()
        {
            var cart = GetCartFromSession();
            return Ok(cart);
        }



        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var product = await Mediator.Send(new GetProductByIdQuery { Id = request.ProductId, JoinCategory=true });
            if (product == null) return NotFound("محصول یافت نشد.");

            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            if (existingItem == null)
            {
                cart.Add(new CartItemDto
                {
                    ProductId = product.Id,
                    Price = product.Price,
                    Quantity = request.Quantity,
                    Product=product
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
