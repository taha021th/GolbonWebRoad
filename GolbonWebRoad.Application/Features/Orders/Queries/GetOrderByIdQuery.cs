using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Orders.Queries
{
    public class GetOrderByIdQuery : IRequest<Order>
    {
        public int Id { get; set; }
    }
    public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdQueryValidator()
        {
            RuleFor(o => o.Id).NotEmpty().WithMessage("شناسه سفارش نمی تواند خالی باشد.");
        }
    }
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Order>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetOrderByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Order> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند دریافت سفارش با شناسه {OrderId}.", request.Id);

            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.Id);

                if (order == null)
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار که خطا نیست
                    _logger.LogWarning("سفارش با شناسه {OrderId} در دیتابیس یافت نشد.", request.Id);
                    return null; // کنترلر این null را به 404 Not Found تبدیل می‌کند
                }

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز
                _logger.LogInformation("سفارش با شناسه {OrderId} متعلق به کاربر {UserId} با موفقیت یافت شد.", request.Id, order.UserId);

                return order;
            }
            catch (Exception ex)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده (مثلاً خطای دیتابیس)
                _logger.LogCritical(ex, "خطای بحرانی در هنگام دریافت سفارش با شناسه {OrderId} از دیتابیس.", request.Id);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}