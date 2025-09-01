using FluentValidation;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Interfaces;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Orders.Commands
{
    public class UpdateOrderStatusCommand : IRequest
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
    }
    public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
    {
        public UpdateOrderStatusCommandValidator()
        {
            RuleFor(o => o.OrderId).NotEmpty().WithMessage("شناسه سفارش نمی تواند خالی باشد.");
            RuleFor(o => o.OrderStatus).NotEmpty().WithMessage("وضعیت سفارش نمی تواند خالی باشد.");
        }
    }
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateOrderStatusCommandHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public UpdateOrderStatusCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateOrderStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات با جزئیات کامل
            _logger.LogInformation(
                "شروع فرآیند تغییر وضعیت سفارش {OrderId} به '{OrderStatus}'.",
                request.OrderId, request.OrderStatus);

            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                {
                    // لاگ هشدار: ثبت یک رویداد قابل انتظار اما ناموفق
                    _logger.LogWarning(
                        "سفارش با شناسه {OrderId} برای تغییر وضعیت یافت نشد.",
                        request.OrderId);

                    throw new NotFoundException("سفارش با این شناسه یافت نشد.");
                }

                var oldStatus = order.OrderStatus;
                order.OrderStatus = request.OrderStatus;

                _unitOfWork.OrderRepository.Update(order); // این متد باید async باشد یا در UnitOfWork مدیریت شود
                await _unitOfWork.CompleteAsync();

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز عملیات
                _logger.LogInformation(
                    "وضعیت سفارش {OrderId} از '{OldStatus}' به '{NewStatus}' با موفقیت تغییر کرد.",
                    request.OrderId, oldStatus, request.OrderStatus);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده
                _logger.LogCritical(ex,
                    "خطای بحرانی در هنگام تغییر وضعیت سفارش {OrderId} به '{OrderStatus}'.",
                    request.OrderId, request.OrderStatus);

                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}