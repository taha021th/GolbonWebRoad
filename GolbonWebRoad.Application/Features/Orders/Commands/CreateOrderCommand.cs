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
        public int? AddressId { get; set; }
        public string? NewFullName { get; set; }
        public string? NewAddressLine { get; set; }
        public string? NewCity { get; set; }
        public string? NewPostalCode { get; set; }
        public string? NewPhone { get; set; }
    }

    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(o => o.UserId).NotEmpty().WithMessage("شناسه کاربر نمی تواند خالی باشد");
            RuleFor(o => o.CartItems).NotEmpty().WithMessage("آیتم سبد خرید نمی تواند خالی باشد.");
            RuleFor(o => o).Must(o => o.AddressId.HasValue || (!string.IsNullOrWhiteSpace(o.NewFullName) && !string.IsNullOrWhiteSpace(o.NewAddressLine) && !string.IsNullOrWhiteSpace(o.NewCity) && !string.IsNullOrWhiteSpace(o.NewPostalCode) && !string.IsNullOrWhiteSpace(o.NewPhone)))
                .WithMessage("یکی از آدرس‌های موجود یا آدرس جدید باید ارسال شود.");
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
            _logger.LogInformation("شروع فرآیند ایجاد سفارش برای کاربر {UserId} با {ItemCount} آیتم.", request.UserId, request.CartItems.Count);

            try
            {
                var orderItems = new List<OrderItem>();
                decimal totalAmount = 0;

                foreach (var item in request.CartItems)
                {
                    // اگر واریانتی برای آیتم سبد خرید مشخص نشده بود، از آن صرف نظر کن
                    if (!item.VariantId.HasValue || item.VariantId.Value == 0)
                    {
                        _logger.LogWarning("آیتم سبد خرید برای محصول {ProductId} فاقد شناسه واریانت معتبر است. از این آیتم صرف نظر می‌شود.", item.ProductId);
                        continue;
                    }

                    _logger.LogDebug("پردازش واریانت محصول {VariantId} با تعداد {Quantity} برای سفارش.", item.VariantId.Value, item.Quantity);

                    // واریانت را به همراه اطلاعات محصول اصلی از دیتابیس بخوان
                    var variant = await _unitOfWork.ProductVariantRepository.GetByIdWithProductAsync(item.VariantId.Value);
                    if (variant == null)
                    {
                        _logger.LogWarning("واریانت محصول با شناسه {VariantId} یافت نشد. از این آیتم صرف نظر می‌شود.", item.VariantId.Value);
                        continue;
                    }

                    // موجودی انبار واریانت را چک کن
                    if (variant.StockQuantity >= item.Quantity)
                    {
                        var price = variant.Price;
                        totalAmount += price * item.Quantity;
                        orderItems.Add(new OrderItem
                        {
                            ProductVariantId = variant.Id,
                            Quantity = item.Quantity,
                            Price = price
                        });

                        // از موجودی انبار کم کن
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

                int? addressId = request.AddressId;
                if (!addressId.HasValue)
                {
                    var addr = new UserAddress
                    {
                        UserId = request.UserId,
                        FullName = request.NewFullName!,
                        Phone = request.NewPhone!,
                        AddressLine = request.NewAddressLine!,
                        City = request.NewCity!,
                        PostalCode = request.NewPostalCode!,
                        IsDefault = false
                    };
                    var created = await _unitOfWork.UserAddressRepository.AddAsync(addr);
                    addressId = created.Id;
                }

                var order = new Order
                {
                    UserId = request.UserId,
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = "در حال پردازش",
                    OrderItems = orderItems,
                    TotalAmount = totalAmount,
                    AddressId = addressId
                };

                var newOrder = _unitOfWork.OrderRepository.Add(order);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("سفارش با شناسه {OrderId} برای کاربر {UserId} با موفقیت ایجاد شد.", newOrder.Id, request.UserId);

                return newOrder.Id;
            }
            catch (Exception ex) when (ex is not InsufficientStockException and not BadRequestException)
            {
                _logger.LogCritical(ex, "خطای پیش‌بینی نشده در هنگام ایجاد سفارش برای کاربر {UserId}.", request.UserId);
                throw;
            }
        }
    }
}