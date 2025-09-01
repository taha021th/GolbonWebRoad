using FluentValidation;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Interfaces;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.CartItems.Commands
{
    public class AddToCartCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
    }
    public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartCommandValidator()
        {
            RuleFor(c => c.UserId).NotEmpty().WithMessage("شناسه کاربر نمی تواند خالی باشد.");
            RuleFor(c => c.ProductId).NotEmpty().WithMessage("شناسه محصول نمی تواند خالی باشد.");
        }
    }

    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddToCartCommandHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را تزریق کرده و وابستگی به IMapper را حذف کنید
        public AddToCartCommandHandler(IUnitOfWork unitOfWork, ILogger<AddToCartCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند افزودن محصول {ProductId} به سبد خرید کاربر {UserId}.",
                request.ProductId, request.UserId);

            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    // لاگ خطا: ثبت یک خطای کسب‌وکار مشخص
                    _logger.LogError("محصول با شناسه {ProductId} برای افزودن به سبد خرید کاربر {UserId} یافت نشد.",
                        request.ProductId, request.UserId);
                    throw new NotFoundException("محصول یافت نشد.");
                }

                var cartItem = await _unitOfWork.CartItemRepository.GetCartItemAsync(request.UserId, request.ProductId);

                if (cartItem != null)
                {
                    cartItem.Quantity += 1;
                    _unitOfWork.CartItemRepository.UpdateCartItem(cartItem); // این متد باید async باشد
                    _logger.LogInformation("تعداد محصول {ProductId} در سبد خرید کاربر {UserId} به {NewQuantity} افزایش یافت.",
                        request.ProductId, request.UserId, cartItem.Quantity);
                }
                else
                {
                    // ساخت دستی موجودیت برای حذف وابستگی غیرضروری به AutoMapper
                    var newCartItem = new CartItem
                    {
                        UserId = request.UserId,
                        ProductId = request.ProductId,
                        Quantity = 1 // همیشه با ۱ شروع می‌شود
                    };
                    _unitOfWork.CartItemRepository.AddCartItem(newCartItem); // این متد باید async باشد
                    _logger.LogInformation("محصول جدید {ProductId} با تعداد 1 به سبد خرید کاربر {UserId} اضافه شد.",
                        request.ProductId, request.UserId);
                }

                await _unitOfWork.CompleteAsync();

                // لاگ اطلاعاتی: ثبت موفقیت نهایی
                _logger.LogInformation("عملیات سبد خرید برای محصول {ProductId} و کاربر {UserId} با موفقیت در دیتابیس ذخیره شد.",
                    request.ProductId, request.UserId);

                return Unit.Value;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده
                _logger.LogCritical(ex, "خطای بحرانی در هنگام افزودن محصول {ProductId} به سبد خرید کاربر {UserId}.",
                    request.ProductId, request.UserId);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}