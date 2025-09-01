using FluentValidation;
using GolbonWebRoad.Application.Interfaces;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.CartItems.Commands
{
    public class RemoveAllCartItemsCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
    }

    public class RemoveAllCartItemsCommandValidator : AbstractValidator<RemoveAllCartItemsCommand>
    {
        public RemoveAllCartItemsCommandValidator()
        {
            RuleFor(c => c.UserId).NotEmpty().WithMessage("شناسه کاربر نمی تواند خالی باشد.");
        }
    }

    public class RemoveAllCartItemsCommandHandler : IRequestHandler<RemoveAllCartItemsCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoveAllCartItemsCommandHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public RemoveAllCartItemsCommandHandler(IUnitOfWork unitOfWork, ILogger<RemoveAllCartItemsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // ۴. امضای متد را به حالت استاندارد و خوانا تغییر دهید
        public async Task<Unit> Handle(RemoveAllCartItemsCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند حذف تمام آیتم‌های سبد خرید برای کاربر {UserId}.", request.UserId);

            try
            {
                var cartItems = await _unitOfWork.CartItemRepository.GetCartItemsByUserIdAsync(request.UserId);

                // ✅ بهبود منطق: بررسی اینکه آیا سبد خرید اصلاً آیتمی برای حذف دارد یا نه
                if (cartItems == null || !cartItems.Any())
                {
                    // لاگ هشدار: این یک خطا نیست، بلکه یک وضعیت قابل انتظار است
                    _logger.LogWarning("سبد خرید کاربر {UserId} از قبل خالی بود. هیچ عملیاتی برای حذف انجام نشد.", request.UserId);
                    return Unit.Value; // عملیات با موفقیت (بدون هیچ کاری) به پایان رسید
                }

                var itemCount = cartItems.Count();
                _unitOfWork.CartItemRepository.RemoveAllCartItem(cartItems); // این متد باید async باشد
                await _unitOfWork.CompleteAsync();

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز عملیات
                _logger.LogInformation("تعداد {ItemCount} آیتم از سبد خرید کاربر {UserId} با موفقیت حذف شد.", itemCount, request.UserId);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده
                _logger.LogCritical(ex, "خطای بحرانی در هنگام حذف تمام آیتم‌های سبد خرید کاربر {UserId}.", request.UserId);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}