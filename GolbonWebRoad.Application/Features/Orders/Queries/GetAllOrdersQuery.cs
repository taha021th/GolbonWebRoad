using AutoMapper;
using GolbonWebRoad.Application.Dtos.Orders;
using GolbonWebRoad.Application.Interfaces;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Orders.Queries
{
    public class GetAllOrdersQuery : IRequest<IEnumerable<OrderDto>>
    {

    }

    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllOrdersQueryHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public GetAllOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllOrdersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند دریافت تمام سفارشات.");

            try
            {
                var orders = await _unitOfWork.OrderRepository.GetAllAsync();

                if (orders == null || !orders.Any())
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار
                    _logger.LogWarning("هیچ سفارشی در سیستم یافت نشد.");
                    return new List<OrderDto>(); // همیشه یک لیست خالی برگردانید، نه null
                }

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز
                _logger.LogInformation("تعداد {OrderCount} سفارش با موفقیت از دیتابیس دریافت شد.", orders.Count());

                return _mapper.Map<IEnumerable<OrderDto>>(orders);
            }
            catch (Exception ex)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده (مثلاً خطای دیتابیس)
                _logger.LogCritical(ex, "خطای بحرانی در هنگام دریافت تمام سفارشات از دیتابیس.");
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}