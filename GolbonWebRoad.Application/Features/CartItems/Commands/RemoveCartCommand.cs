using FluentValidation;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Interfaces;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.CartItems.Commands
{
    public class RemoveCartCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
    }
    public class RemoveCartCommandValidator : AbstractValidator<RemoveCartCommand>
    {
        public RemoveCartCommandValidator()
        {
            RuleFor(c => c.UserId).NotEmpty().WithMessage("شناسه کاربر نمی تواند خالی باشد.");
            RuleFor(c => c.ProductId).NotEmpty().WithMessage("شناسه محصول نمی تواند خالی باشد.");
        }
    }
    public class RemoveCartCommandHandler : IRequestHandler<RemoveCartCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoveCartCommandHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public RemoveCartCommandHandler(IUnitOfWork unitOfWork, ILogger<RemoveCartCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveCartCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند حذف محصول {ProductId} از سبد خرید کاربر {UserId}.",
                request.ProductId, request.UserId);

            try
            {
                var cartItem = await _unitOfWork.CartItemRepository.GetCartItemAsync(request.UserId, request.ProductId);
                if (cartItem == null)
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار که خطا نیست
                    _logger.LogWarning("آیتم محصول {ProductId} برای حذف در سبد خرید کاربر {UserId} یافت نشد.",
                        request.ProductId, request.UserId);
                    throw new NotFoundException("آیتم مورد نظر در سبد خرید یافت نشد.");
                }

                _unitOfWork.CartItemRepository.RemoveCartItem(cartItem); // این متد باید async باشد
                await _unitOfWork.CompleteAsync();

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز عملیات
                _logger.LogInformation("محصول {ProductId} از سبد خرید کاربر {UserId} با موفقیت حذف شد.",
                    request.ProductId, request.UserId);

                return Unit.Value;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده
                _logger.LogCritical(ex, "خطای بحرانی در هنگام حذف محصول {ProductId} از سبد خرید کاربر {UserId}.",
                    request.ProductId, request.UserId);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}