using AutoMapper;
using GolbonWebRoad.Application.Features.Products.ProductVariants.Queries; // ۳. using برای کوئری
using GolbonWebRoad.Web.Models.Cart;           // ۲. using برای ViewModel ها
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
            var cartItems = GetCart(); // این متد حالا List<CartItemViewModel> برمی‌گرداند
            var cartViewModel = new CartViewModel
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(ci => ci.TotalPrice)
            };
            return View(cartViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id, int? variantId, int quantity = 1)
        {
            if (quantity < 1) quantity = 1;

            if (!variantId.HasValue)
            {
                TempData["ErrorMessage"] = "لطفا نوع محصول را انتخاب کنید.";
                return RedirectToAction("Detail", "Products", new { id = id });
            }

            // ۴. هندلر حالا یک انتیتی کامل برمی‌گرداند
            var variantEntity = await _mediator.Send(new GetByIdProductVariantQuery { Id = variantId.Value });
            if (variantEntity == null)
            {
                return NotFound();
            }

            var cart = GetCart(); // این متد List<CartItemViewModel> برمی‌گرداند
            var existingItem = cart.FirstOrDefault(c => c.ProductId == id && c.VariantId == variantId.Value);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                // ۵. انتیتی را در همینجا به ViewModel مپ می‌کنیم
                var cartItemViewModel = new CartItemViewModel
                {
                    ProductId = variantEntity.ProductId,
                    VariantId = variantEntity.Id,
                    Quantity = quantity,
                    Price = variantEntity.Price,
                    Product = _mapper.Map<ProductCartViewModel>(variantEntity.Product),
                    VariantAttributes = (variantEntity.AttributeValues ?? new List<Domain.Entities.ProductAttributeValue>())
                        .Where(a => a != null && a.Attribute != null)
                        .ToDictionary(a => a.Attribute.Name, a => a.Value)
                };
                cart.Add(cartItemViewModel);
            }

            SaveCart(cart);
            TempData["CartSuccess"] = "محصول به سبد خرید اضافه شد.";
            return RedirectToAction("Detail", "Products", new { id = id });
        }

        public IActionResult RemoveFromCart(int id, int? variantId)
        {
            var cart = GetCart();
            var itemToRemove = cart.FirstOrDefault(c => c.ProductId == id && c.VariantId == variantId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        // ۶. این متدها حالا با List<CartItemViewModel> کار می‌کنند
        private List<CartItemViewModel> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItemViewModel>();
            }
            return JsonSerializer.Deserialize<List<CartItemViewModel>>(cartJson) ?? new List<CartItemViewModel>();
        }

        private void SaveCart(List<CartItemViewModel> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }
}