using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Application.Features.Orders.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace GolbonWebRoad.Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IMediator _mediator;
        private const string CartSessionKey = "Cart";

        public CheckoutController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var cart = GetCart();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!cart.Any() || string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var command = new CreateOrderCommand
            {
                UserId = userId,
                CartItems = cart.Select(c => new CartItemSummaryDto
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Price
                }).ToList()
            };

            var orderId = await _mediator.Send(command);
            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction("Success", new { orderId = orderId });
        }

        public IActionResult Success(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        private List<CartItemDto> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<CartItemDto>() : JsonSerializer.Deserialize<List<CartItemDto>>(cartJson);
        }
    }
}
