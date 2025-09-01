using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Application.Interfaces;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.CartItems.Queries
{
    public class GetCartQuery : IRequest<IEnumerable<CartItemDto>>
    {
        public string UserId { get; set; }
    }

    public class GetCartQueryValidator : AbstractValidator<GetCartQuery>
    {
        public GetCartQueryValidator()
        {
            RuleFor(c => c.UserId).NotEmpty().WithMessage("شناسه کاربر نمی تواند خالی باشد.");
        }
    }

    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, IEnumerable<CartItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCartQueryHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public GetCartQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCartQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CartItemDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند دریافت سبد خرید برای کاربر {UserId}.", request.UserId);

            try
            {
                var cartItems = await _unitOfWork.CartItemRepository.GetCartItemsByUserIdAsync(request.UserId);

                if (cartItems == null || !cartItems.Any())
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار که خطا نیست
                    _logger.LogWarning("هیچ آیتمی در سبد خرید کاربر {UserId} یافت نشد.", request.UserId);
                    return new List<CartItemDto>(); // همیشه یک لیست خالی برگردانید، نه null
                }

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز
                _logger.LogInformation("تعداد {ItemCount} آیتم در سبد خرید کاربر {UserId} با موفقیت یافت شد.", cartItems.Count(), request.UserId);

                return _mapper.Map<IEnumerable<CartItemDto>>(cartItems);
            }
            catch (Exception ex)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده (مثلاً خطای دیتابیس)
                _logger.LogCritical(ex, "خطای بحرانی در هنگام دریافت سبد خرید کاربر {UserId} از دیتابیس.", request.UserId);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}