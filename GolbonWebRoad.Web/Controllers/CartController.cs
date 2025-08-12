using GolbonWebRoad.Application.Dtos;
using GolbonWebRoad.Application.Features.Orders.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace GolbonWebRoad.Web.Controllers
{

    public class CartController : Controller
    {
        private readonly IMediator _mediator;
        private const string CartSessionKey = "Cart";
        public CartController(IMediator mediator)
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

                UserId=userId,
                CartItems=cart.Select(c => new CartItemDto
                {
                    ProductId = c.ProductId,
                    Quantity=c.Quantity,
                    Price=c.Price
                }).ToList()
            };
            var orderId = await _mediator.Send(command);
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction("Success", new { orderid = orderId });

        }
        public IActionResult Success(int orderId)
        {
            ViewBag.OrderId =orderId;
            return View();
        }

        private List<CartItemDto> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItemDto>();
            }
            return JsonSerializer.Deserialize<List<CartItemDto>>(cartJson);
        }
        private void SaveCart(List<CartItemDto> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }
}
