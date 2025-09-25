using AutoMapper;
using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GolbonWebRoad.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private const string CartSessionKey = "Cart";

        public CartController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            // Map product inside each cart item to lightweight ProductCartViewModel
            var mapped = cart.Select(ci => new GolbonWebRoad.Web.Models.Cart.CartItemViewModel
            {
                ProductId = ci.ProductId,
                ColorId = ci.ColorId,
                Quantity = ci.Quantity,
                Price = ci.Price,
                Product = _mapper.Map<GolbonWebRoad.Web.Models.Cart.ProductCartViewModel>(ci.Product)
            }).ToList();
            return View(mapped);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id, int? variantId, int quantity = 1)
        {
            if (quantity < 1) quantity = 1;

            var product = await _mediator.Send(new GetProductByIdQuery
            {
                Id = id,
                JoinImages = true,
                JoinBrand = false,
                JoinCategory = false,
                JoinColors = false,
                JoinReviews = false
            });
            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductDto>(product);

            var cart = GetCart();

            // Determine price and key based on variant
            decimal price = product.Price;
            if (variantId.HasValue)
            {
                // In absence of a separate query, reusing UnitOfWork via mediator is not available here.
                // The UI sends the correct price context; as a simple approach, keep product price.
                // In a fuller implementation, fetch variant price here via a query.
            }

            // key by product and variant selection
            var existing = cart.FirstOrDefault(c => c.ProductId == id && c.VariantId == variantId);

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItemDto
                {
                    ProductId = id,
                    VariantId = variantId,
                    Quantity = quantity,
                    Price = price,
                    Product = productDto
                });
            }

            SaveCart(cart);
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        public IActionResult RemoveFromCart(int id, int? colorId)
        {
            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.ProductId == id && c.ColorId == colorId);
            if (existing != null)
            {
                cart.Remove(existing);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        private List<CartItemDto> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItemDto>();
            }
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