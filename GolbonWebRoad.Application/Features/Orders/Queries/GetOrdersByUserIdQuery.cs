using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Dtos.Orders;
using GolbonWebRoad.Application.Interfaces;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Orders.Queries
{
    public class GetOrdersByUserIdQuery : IRequest<IEnumerable<OrderDto>>
    {
        public string UserId { get; set; }
    }

    public class GetOrdersByUserIdQueryValidator : AbstractValidator<GetOrdersByUserIdQuery>
    {
        public GetOrdersByUserIdQueryValidator()
        {
            RuleFor(o => o.UserId).NotEmpty().WithMessage("شناسه کاربر نمی تواند خالی باشد.");
        }
    }

    public class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, IEnumerable<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOrdersByUserIdQueryHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public GetOrdersByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetOrdersByUserIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند دریافت سفارشات برای کاربر {UserId}.", request.UserId);

            try
            {
                var orders = await _unitOfWork.OrderRepository.GetByUserIdAsync(request.UserId);

                if (orders == null || !orders.Any())
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار که خطا نیست
                    _logger.LogWarning("هیچ سفارشی برای کاربر {UserId} در دیتابیس یافت نشد.", request.UserId);
                    return new List<OrderDto>(); // همیشه یک لیست خالی برگردانید، نه null
                }

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز
                _logger.LogInformation("تعداد {OrderCount} سفارش برای کاربر {UserId} با موفقیت یافت شد.", orders.Count(), request.UserId);

                return _mapper.Map<IEnumerable<OrderDto>>(orders);
            }
            catch (Exception ex)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده (مثلاً خطای دیتابیس)
                _logger.LogCritical(ex, "خطای بحرانی در هنگام دریافت سفارشات کاربر {UserId} از دیتابیس.", request.UserId);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}