using AutoMapper;
using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Application.Features.Orders.Commands;
using GolbonWebRoad.Web.Models.Cart;
using GolbonWebRoad.Web.Models.Transactions;
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
        private readonly IMapper _mapper;
        private const string CartSessionKey = "Cart";

        public CheckoutController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }
            var mapped = cart.Select(ci => new CartItemViewModel
            {
                ProductId = ci.ProductId,
                ColorId = null,
                Quantity = ci.Quantity,
                Price = ci.Price,
                Product = _mapper.Map<ProductCartViewModel>(ci.Product)
            }).ToList();
            ViewBag.TotalAmount = mapped.Sum(i => (long)(i.Price * i.Quantity));
            ViewBag.TempOrderId = Guid.NewGuid().ToString("N");
            return View(mapped);
        }

        [HttpGet]
        //[IgnoreAntiforgeryToken]
        public async Task<IActionResult> PlaceOrder(CallbackMessageViewModel messageViewModel)
        {
            var cart = GetCart();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!cart.Any() || string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }
            if (messageViewModel.PaymentStatus!=true)
            {
                messageViewModel.MessageTransAction="تراکنش ناموفق";
                messageViewModel.MessageCreateOrder="سفارش شما ثبت نشد";
                return RedirectToAction("Success", messageViewModel);
            }

            var command = new CreateOrderCommand
            {
                UserId = userId,
                CartItems = cart.Select(c => new CartItemSummaryDto
                {
                    ProductId = c.ProductId,
                    VariantId = c.VariantId,
                    Quantity = c.Quantity,
                    Price = c.Price
                }).ToList()
            };

            try
            {
                var orderId = await _mediator.Send(command);
                HttpContext.Session.Remove(CartSessionKey);

                messageViewModel.MessageTransAction="تراکنش موفق";
                messageViewModel.MessageCreateOrder="ثبت سفارش با موفقیت انجام شد";

                return RedirectToAction("Success", messageViewModel);
            }
            catch (Exception ex)
            {
                messageViewModel.MessageTransAction="تراکنش موفق";
                messageViewModel.MessageCreateOrder="ثبت سفارش با مشکل مواجه شد";
                return RedirectToAction("Success", messageViewModel);
            }
        }


        [HttpGet]
        public IActionResult Success(CallbackMessageViewModel viewmodel)
        {

            return View(viewmodel);
        }

        private List<CartItemDto> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<CartItemDto>() : JsonSerializer.Deserialize<List<CartItemDto>>(cartJson);
        }
    }
}
