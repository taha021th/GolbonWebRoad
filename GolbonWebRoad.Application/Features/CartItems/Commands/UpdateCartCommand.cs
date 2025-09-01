using FluentValidation;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Interfaces;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.CartItems.Commands
{
    public class UpdateCartItemCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
    {
        public UpdateCartItemCommandValidator()
        {
            RuleFor(c => c.UserId).NotEmpty().WithMessage("شناسه کاربر نمی تواند خالی باشد.");
            RuleFor(c => c.ProductId).NotEmpty().WithMessage("شناسه محصول نمی تواند خالی باشد.");
            RuleFor(c => c.Quantity).NotNull().WithMessage("تعداد محصول نمی تواند خالی باشد."); // NotNull برای int مناسب‌تر است
        }
    }

    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCartItemCommandHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public UpdateCartItemCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCartItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند به‌روزرسانی محصول {ProductId} در سبد خرید کاربر {UserId} به تعداد {Quantity}.",
                request.ProductId, request.UserId, request.Quantity);

            try
            {
                var cartItem = await _unitOfWork.CartItemRepository.GetCartItemAsync(request.UserId, request.ProductId);
                if (cartItem == null)
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار
                    _logger.LogWarning("آیتم محصول {ProductId} برای به‌روزرسانی در سبد خرید کاربر {UserId} یافت نشد.",
                        request.ProductId, request.UserId);
                    throw new NotFoundException("آیتم مورد نظر در سبد خرید یافت نشد.");
                }

                if (request.Quantity <= 0)
                {
                    _unitOfWork.CartItemRepository.RemoveCartItem(cartItem);
                    _logger.LogInformation("محصول {ProductId} از سبد خرید کاربر {UserId} حذف شد (تعداد درخواستی: {Quantity}).",
                        request.ProductId, request.UserId, request.Quantity);
                }
                else
                {
                    var oldQuantity = cartItem.Quantity;
                    cartItem.Quantity = request.Quantity;
                    _unitOfWork.CartItemRepository.UpdateCartItem(cartItem);
                    _logger.LogInformation("تعداد محصول {ProductId} در سبد خرید کاربر {UserId} از {OldQuantity} به {NewQuantity} تغییر یافت.",
                        request.ProductId, request.UserId, oldQuantity, request.Quantity);
                }

                await _unitOfWork.CompleteAsync();

                return Unit.Value;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده
                _logger.LogCritical(ex, "خطای بحرانی در هنگام به‌روزرسانی سبد خرید برای کاربر {UserId} و محصول {ProductId}.",
                    request.UserId, request.ProductId);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}