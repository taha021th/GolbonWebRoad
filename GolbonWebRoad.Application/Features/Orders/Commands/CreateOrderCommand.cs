using FluentValidation;
using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<int>
    {
        public string UserId { get; set; }
        public List<CartItemSummaryDto> CartItems { get; set; }
    }

    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(o => o.UserId).NotEmpty().WithMessage("شناسه کاربر نمی تواند خالی باشد");
            RuleFor(o => o.CartItems).NotEmpty().WithMessage("آیتم سبد خرید نمی تواند خالی باشد.");
        }
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateOrderCommandHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع یک عملیات مهم
            _logger.LogInformation("شروع فرآیند ایجاد سفارش برای کاربر {UserId} با {ItemCount} آیتم.", request.UserId, request.CartItems.Count);

            try
            {
                var orderItems = new List<OrderItem>();
                decimal totalAmount = 0;

                foreach (var item in request.CartItems)
                {
                    // لاگ دیباگ: ثبت جزئیات فنی برای توسعه‌دهندگان
                    _logger.LogDebug("پردازش محصول {ProductId} با تعداد {Quantity} برای سفارش کاربر {UserId}.", item.ProductId, item.Quantity, request.UserId);

                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        // لاگ هشدار: ثبت یک رویداد غیرمنتظره اما قابل مدیریت
                        _logger.LogWarning("محصول با شناسه {ProductId} برای سفارش کاربر {UserId} یافت نشد. از این آیتم صرف نظر می‌شود.", item.ProductId, request.UserId);
                        continue;
                    }

                    if (product.Quantity >= item.Quantity)
                    {
                        var price = product.Price;
                        totalAmount += price * item.Quantity;
                        orderItems.Add(new OrderItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Price = price // قیمت از دیتابیس خوانده می‌شود
                        });
                        product.Quantity -= item.Quantity;
                        _unitOfWork.ProductRepository.Update(product); // این متد باید async باشد
                    }
                    else
                    {
                        // لاگ خطا: ثبت یک خطای کسب‌وکار مشخص که باعث شکست عملیات شده
                        _logger.LogError("موجودی محصول {ProductId} ({ProductName}) برای کاربر {UserId} کافی نیست. موجودی: {Stock}, درخواست: {Quantity}",
                            product.Id, product.Name, request.UserId, product.Quantity, item.Quantity);

                        // استفاده از Exception سفارشی و استاندارد
                        throw new InsufficientStockException($"تعداد درخواست شما برای محصول {product.Name} بیشتر از موجودی محصول می باشد. تعداد باقی مانده از محصول :{product.Quantity} درخواست شما: {item.Quantity}");
                    }

                }

                if (!orderItems.Any())
                {
                    _logger.LogWarning("هیچ محصول معتبری برای ایجاد سفارش برای کاربر {UserId} یافت نشد. عملیات لغو شد.", request.UserId);
                    // شما می‌توانید یک Exception سفارشی برای این حالت پرتاب کنید
                    throw new BadRequestException("هیچکدام از محصولات موجود در سبد خرید شما، معتبر یا دارای موجودی کافی نبودند.");
                }

                var order = new Order
                {
                    UserId = request.UserId,
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = "در حال پردازش",
                    OrderItems = orderItems,
                    TotalAmount = totalAmount
                };

                var newOrder = _unitOfWork.OrderRepository.Add(order); // متد باید async باشد
                await _unitOfWork.CompleteAsync();

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز عملیات
                _logger.LogInformation("سفارش با شناسه {OrderId} برای کاربر {UserId} با مبلغ کل {TotalAmount} با موفقیت ایجاد شد.", newOrder.Id, request.UserId, totalAmount);

                return newOrder.Id;
            }
            catch (Exception ex) when (ex is not InsufficientStockException and not BadRequestException)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده که نیاز به بررسی فوری توسط توسعه‌دهنده دارد
                _logger.LogCritical(ex, "خطای بحرانی و پیش‌بینی نشده در هنگام ایجاد سفارش برای کاربر {UserId}.", request.UserId);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}