using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Application.Features.Orders.Commands;
using GolbonWebRoad.Application.Features.Products.Queries;

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

        // Your existing Index action - it's perfect.
        public IActionResult Index()
        {
            var cart = GetCart();
            // This is the view we designed in the previous step.
            return View(cart);
        }

        // ==========================================================
        // === ADDING THE MISSING ACTIONS TO YOUR CONTROLLER      ===
        // ==========================================================

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var product = await _mediator.Send(new GetProductByIdQuery { Id = id });
            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                // Note: Your CartItemDto has a `Price` property. Let's populate it.
                cart.Add(new CartItemDto
                {
                    ProductId = id,
                    Quantity = quantity,
                    Price = product.Price, // Populating the price
                    Product = product      // Populating the product details for the view
                });
            }

            SaveCart(cart);
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        // ==========================================================
        // === YOUR EXISTING ACTIONS AND HELPERS - NO CHANGES NEEDED ===
        // ==========================================================

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var cart = GetCart();
            // For this to work, the user MUST be logged in.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!cart.Any() || string.IsNullOrEmpty(userId))
            {
                // If not logged in, maybe redirect to login page?
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
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItemDto>();
            }
            // We need to handle the Product property which might not be in the session
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<CartItemDto>>(cartJson, options) ?? new List<CartItemDto>();
        }

        private void SaveCart(List<CartItemDto> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }
}