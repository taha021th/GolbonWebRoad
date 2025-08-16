using GolbonWebRoad.Application.Dtos;
using GolbonWebRoad.Application.Features.Orders.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace GolbonWebRoad.Api.Controllers
{

    [Authorize]
    public class CheckoutController : ApiBaseController
    {

        private const string CartSessionKey = "Cart";



        // POST: api/checkout/placeorder
        [HttpPost("placeorder")]
        public async Task<IActionResult> PlaceOrder()
        {
            // گرفتن شناسه کاربر از توکن احراز هویت او
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("کاربر شناسایی نشد.");
            }

            // خواندن سبد خرید از سشن
            var cart = GetCartFromSession();
            if (!cart.Any())
            {
                return BadRequest("سبد خرید شما خالی است.");
            }

            // ساخت کامند برای ثبت سفارش
            var command = new CreateOrderCommand
            {
                UserId = userId,
                CartItems = cart.Select(c => new CartItemDto
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Price
                }).ToList()
            };

            // ارسال کامند به لایه اپلیکیشن و گرفتن شناسه سفارش جدید
            var orderId = await Mediator.Send(command);

            // خالی کردن سبد خرید پس از ثبت موفق سفارش
            HttpContext.Session.Remove(CartSessionKey);

            // برگرداندن یک پاسخ موفق به همراه شناسه سفارش
            return Ok(new { OrderId = orderId, Message = "سفارش شما با موفقیت ثبت شد." });
        }

        private List<CartItemDto> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItemDto>()
                : JsonSerializer.Deserialize<List<CartItemDto>>(cartJson);
        }
    }
}
