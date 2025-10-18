using AutoMapper;
using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Application.Features.Orders.Commands;
using GolbonWebRoad.Application.Features.Users.Queries;
using GolbonWebRoad.Application.Interfaces.Services.Logistics;
using GolbonWebRoad.Web.Models.Cart;
using GolbonWebRoad.Web.Models.Checkout;
using GolbonWebRoad.Application.Dtos.Logistics;
using GolbonWebRoad.Domain.Entities;
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
        private readonly ILogisticsService _logisticsService;
        private const string CartSessionKey = "Cart";

        public CheckoutController(IMediator mediator, IMapper mapper, ILogisticsService logisticsService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logisticsService=logisticsService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }
            var mapped = cart.Select(ci => new CartItemViewModel
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                Price = ci.Price,
                Product = _mapper.Map<ProductCartViewModel>(ci.Product)
            }).ToList();
            
            var totalAmount = mapped.Sum(i => (long)(i.Price * i.Quantity));
            var viewModel = new CheckoutViewModel
            {
                CartItems = mapped,
                TotalAmount = totalAmount
            };

            // Load user addresses if logged in
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var addresses = await _mediator.Send(new GetUserAddressesQuery { UserId = userId });
                viewModel.UserAddresses = addresses?.ToList() ?? new List<GolbonWebRoad.Domain.Entities.UserAddress>();

                // اگر کاربر آدرس دارد، گزینه های ارسال رو محاسبه کن
                if (viewModel.UserAddresses.Any())
                {
                    try
                    {
                        var defaultAddress = viewModel.UserAddresses.FirstOrDefault(a => a.IsDefault) 
                                            ?? viewModel.UserAddresses.First();
                        
                        viewModel.ShippingOptions = await GetShippingOptionsForCart(cart, defaultAddress);
                    }
                    catch (Exception ex)
                    {
                        // در صورت خطا، گزینه های ارسال خالی می ماند
                        TempData["ShippingError"] = "خطا در دریافت گزینه های ارسال. لطفاً دوباره تلاش کنید.";
                    }
                }
            }

            ViewBag.TempOrderId = Guid.NewGuid().ToString("N");
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            // اعتبارسنجی مدل
            if (!ModelState.IsValid)
            {
                // در صورت خطای اعتبارسنجی، دوباره صفحه را با خطاها نمایش بده
                var cart = GetCart();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                model.CartItems = cart.Select(ci => new CartItemViewModel
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Price,
                    Product = _mapper.Map<ProductCartViewModel>(ci.Product)
                }).ToList();
                model.TotalAmount = cart.Sum(i => (long)(i.Price * i.Quantity));
                
                if (!string.IsNullOrEmpty(userId))
                {
                    var addresses = await _mediator.Send(new GetUserAddressesQuery { UserId = userId });
                    model.UserAddresses = addresses?.ToList() ?? new List<GolbonWebRoad.Domain.Entities.UserAddress>();
                }
                
                return View("Index", model);
            }

            // پردازش اطلاعات روش ارسال انتخاب شده
            var shippingParts = model.SelectedShippingMethod.Split('|');
            if (shippingParts.Length != 2 || !decimal.TryParse(shippingParts[1], out decimal shippingCost))
            {
                TempData["CheckoutError"] = "روش ارسال انتخاب شده معتبر نیست.";
                return RedirectToAction(nameof(Index));
            }
            
            var shippingMethod = shippingParts[0];

            // شبیه سازی پرداخت موفق (در اینجا فرض می کنیم پرداخت موفق بوده)
            // در پروژه واقعی، پس از بازگشت از درگاه پرداخت این متد فراخوانی می شود
            return await ProcessSuccessfulPayment(model, shippingMethod, shippingCost);
        }
        
        private async Task<IActionResult> ProcessSuccessfulPayment(CheckoutViewModel model, string shippingMethod, decimal shippingCost)
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
                    VariantId = c.VariantId,
                    Quantity = c.Quantity,
                    Price = c.Price
                }).ToList(),
                AddressId = model.SelectedAddressId,
                NewAddressLine = model.SelectedAddressId.HasValue ? null : model.NewAddressLine,
                NewCity = model.SelectedAddressId.HasValue ? null : model.NewCity,
                NewProvince = model.SelectedAddressId.HasValue ? null : model.NewProvince,
                NewPostalCode = model.SelectedAddressId.HasValue ? null : model.NewPostalCode,
                NewPhone = model.SelectedAddressId.HasValue ? null : model.NewPhone,
                ShippingMethod = shippingMethod,
                ShippingCost = shippingCost
            };

            try
            {
                var orderId = await _mediator.Send(command);
                HttpContext.Session.Remove(CartSessionKey);

                var successModel = new CallbackMessageViewModel
                {
                    PaymentStatus = true,
                    MessageTransAction = "تراکنش موفق",
                    MessageCreateOrder = "ثبت سفارش با موفقیت انجام شد"
                };

                return RedirectToAction("Success", successModel);
            }
            catch (Exception ex)
            {
                var errorModel = new CallbackMessageViewModel
                {
                    PaymentStatus = false,
                    MessageTransAction = "تراکنش موفق",
                    MessageCreateOrder = "ثبت سفارش با مشکل مواجه شد"
                };
                return RedirectToAction("Success", errorModel);
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

        private async Task<List<ShippingQuoteDto>> GetShippingOptionsForCart(List<CartItemDto> cart, UserAddress address)
        {
            // محاسبه وزن و ابعاد کل سبد خرید
            double totalWeight = 0;
            double totalVolume = 0;

            foreach (var item in cart)
            {
                // فرض می‌کنیم هر محصول ۰.۵ کیلو وزن دارد (می‌تونید از دیتابیس بخونید)
                totalWeight += item.Quantity * 0.5;
                // فرض می‌کنیم هر محصول ۲۰×۱۵×۱۰ سانتیمتر حجم دارد
                totalVolume += item.Quantity * (20 * 15 * 10);
            }

            // تخمین ابعاد بسته بر اساس حجم کل
            var estimatedLength = Math.Pow(totalVolume, 1.0 / 3.0) * 1.5;
            var estimatedWidth = Math.Pow(totalVolume, 1.0 / 3.0) * 1.2;
            var estimatedHeight = Math.Pow(totalVolume, 1.0 / 3.0);

            var shipmentDetails = new ShipmentDetailsDto
            {
                DestinationAddress = new AddressDto
                {
                    Province = address.Province,
                    City = address.City,
                    PostalCode = address.PostalCode,
                    FullAddress = address.AddressLine
                },
                WeightInKg = totalWeight,
                LengthInCm = estimatedLength,
                WidthInCm = estimatedWidth,
                HeightInCm = estimatedHeight
            };

            return await _logisticsService.GetShippingQuotes(shipmentDetails);
        }
    }
}
