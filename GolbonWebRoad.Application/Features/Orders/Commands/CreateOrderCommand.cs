using FluentValidation;
using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Orders.Commands
{
    /// <summary>
    /// کامند (دستور) کامل برای ایجاد یک سفارش جدید.
    /// این کلاس تمام اطلاعات لازم، از جمله آیتم‌های سبد خرید، آدرس و روش ارسال را نگهداری می‌کند.
    /// </summary>
    public class CreateOrderCommand : IRequest<int>
    {
        public string UserId { get; set; }
        public List<CartItemSummaryDto> CartItems { get; set; }

        // بخش آدرس: کاربر می‌تواند یک آدرس موجود را انتخاب کند یا یک آدرس جدید وارد کند
        public int? AddressId { get; set; }
        public string? NewFullName { get; set; }
        public string? NewAddressLine { get; set; }
        public string? NewCity { get; set; }
        public string? NewProvince { get; set; }
        public string? NewPostalCode { get; set; }
        public string? NewPhone { get; set; }

        // ==========================================================
        // === پراپرتی‌های اضافه شده برای دریافت اطلاعات ارسال ===
        // ==========================================================
        public string ShippingMethod { get; set; }
        public decimal ShippingCost { get; set; }
    }

    /// <summary>
    /// کلاس اعتبارسنجی برای کامند ایجاد سفارش با استفاده از FluentValidation.
    /// </summary>
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(o => o.UserId).NotEmpty().WithMessage("شناسه کاربر نمی تواند خالی باشد");
            RuleFor(o => o.CartItems).NotEmpty().WithMessage("سبد خرید نمی تواند خالی باشد.");

            // بررسی می‌کند که یا یک آدرس موجود انتخاب شده باشد، یا تمام فیلدهای آدرس جدید پر شده باشند.
            RuleFor(o => o).Must(o => o.AddressId.HasValue ||
                                      (!string.IsNullOrWhiteSpace(o.NewFullName) &&
                                       !string.IsNullOrWhiteSpace(o.NewAddressLine) &&
                                       !string.IsNullOrWhiteSpace(o.NewCity) &&
                                       !string.IsNullOrWhiteSpace(o.NewProvince) &&
                                       !string.IsNullOrWhiteSpace(o.NewPostalCode) &&
                                       !string.IsNullOrWhiteSpace(o.NewPhone)))
                .WithMessage("یک آدرس موجود یا یک آدرس جدید باید به صورت کامل وارد شود.");

            // اعتبارسنجی برای فیلدهای ارسال
            RuleFor(o => o.ShippingMethod).NotEmpty().WithMessage("روش ارسال نمی‌تواند خالی باشد.");
            RuleFor(o => o.ShippingCost).GreaterThanOrEqualTo(0).WithMessage("هزینه ارسال نامعتبر است.");
        }
    }

    /// <summary>
    /// هندلر (پردازشگر) کامند ایجاد سفارش.
    /// این کلاس منطق اصلی کسب‌وکار برای ثبت سفارش را اجرا می‌کند:
    /// ۱. بررسی موجودی انبار
    /// ۲. کسر از موجودی
    /// ۳. ایجاد آدرس جدید در صورت نیاز
    /// ۴. محاسبه قیمت نهایی
    /// ۵. ثبت سفارش در دیتابیس
    /// </summary>
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع فرآیند ایجاد سفارش برای کاربر {UserId} با {ItemCount} آیتم.", request.UserId, request.CartItems.Count);

            try
            {
                var orderItems = new List<OrderItem>();
                decimal itemsTotalAmount = 0; // مبلغ کل آیتم‌ها بدون هزینه ارسال

                foreach (var item in request.CartItems)
                {
                    if (!item.VariantId.HasValue || item.VariantId.Value == 0)
                    {
                        _logger.LogWarning("آیتم سبد خرید برای محصول {ProductId} فاقد شناسه واریانت معتبر است. از این آیتم صرف نظر می‌شود.", item.ProductId);
                        continue;
                    }

                    _logger.LogDebug("پردازش واریانت محصول {VariantId} با تعداد {Quantity} برای سفارش.", item.VariantId.Value, item.Quantity);
                    var variant = await _unitOfWork.ProductVariantRepository.GetByIdWithProductAsync(item.VariantId.Value);

                    if (variant == null)
                    {
                        _logger.LogWarning("واریانت محصول با شناسه {VariantId} یافت نشد. از این آیتم صرف نظر می‌شود.", item.VariantId.Value);
                        continue;
                    }

                    if (variant.StockQuantity >= item.Quantity)
                    {
                        var price = variant.Price;
                        itemsTotalAmount += price * item.Quantity;
                        orderItems.Add(new OrderItem
                        {
                            ProductVariantId = variant.Id,
                            Quantity = item.Quantity,
                            Price = price
                        });

                        variant.StockQuantity -= item.Quantity;
                        _unitOfWork.ProductVariantRepository.Update(variant);
                    }
                    else
                    {
                        _logger.LogError("موجودی واریانت محصول {VariantId} ({ProductName}) کافی نیست. موجودی: {Stock}, درخواست: {Quantity}",
                            variant.Id, variant.Product.Name, variant.StockQuantity, item.Quantity);
                        throw new InsufficientStockException($"موجودی محصول {variant.Product.Name} کافی نیست. موجودی فعلی: {variant.StockQuantity}، درخواست شما: {item.Quantity}");
                    }
                }

                if (!orderItems.Any())
                {
                    _logger.LogWarning("هیچ محصول معتبری برای ایجاد سفارش برای کاربر {UserId} یافت نشد.", request.UserId);
                    throw new BadRequestException("هیچکدام از محصولات موجود در سبد خرید شما، معتبر یا دارای موجودی کافی نبودند.");
                }

                // مدیریت آدرس: اگر آدرس جدیدی ارسال شده بود، آن را در دیتابیس ذخیره کن
                int addressId = request.AddressId.GetValueOrDefault();
                if (addressId == 0)
                {
                    var newAddress = new UserAddress
                    {
                        UserId = request.UserId,
                        FullName = request.NewFullName!,
                        Phone = request.NewPhone!,
                        AddressLine = request.NewAddressLine!,
                        City = request.NewCity!,
                        Province = request.NewProvince!,
                        PostalCode = request.NewPostalCode!,
                        IsDefault = false // آدرس جدید به عنوان پیش‌فرض ذخیره نمی‌شود
                    };
                    var createdAddress = await _unitOfWork.UserAddressRepository.AddAsync(newAddress);
                    addressId = createdAddress.Id;
                    _logger.LogInformation("آدرس جدید با شناسه {AddressId} برای کاربر {UserId} ایجاد شد.", addressId, request.UserId);
                }

                // محاسبه قیمت نهایی سفارش (مبلغ آیتم‌ها + هزینه ارسال)
                var finalTotalAmount = itemsTotalAmount + request.ShippingCost;

                var order = new Order
                {
                    UserId = request.UserId,
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = "در حال پردازش",
                    OrderItems = orderItems,
                    TotalAmount = finalTotalAmount,
                    AddressId = addressId,
                    ShippingMethod = request.ShippingMethod,
                    ShippingCost = request.ShippingCost
                };

                var createdOrder = _unitOfWork.OrderRepository.Add(order);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("سفارش با شناسه {OrderId} برای کاربر {UserId} با موفقیت ایجاد شد.", createdOrder.Id, request.UserId);

                return createdOrder.Id;
            }
            catch (Exception ex) when (ex is not InsufficientStockException and not BadRequestException)
            {
                _logger.LogCritical(ex, "خطای پیش‌بینی نشده در هنگام ایجاد سفارش برای کاربر {UserId}.", request.UserId);
                throw; // خطا را دوباره پرتاب کن تا لایه‌های بالاتر آن را مدیریت کنند
            }
        }
    }
}

