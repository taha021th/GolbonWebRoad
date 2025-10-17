using GolbonWebRoad.Application.Dtos.Logistics;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Interfaces.Services.Logistics;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Orders.Commands
{
    /// <summary>
    /// کامند برای ثبت نهایی یک مرسوله در سیستم شرکت پستی و دریافت کد رهگیری.
    /// </summary>
    public class CreateShipmentCommand : IRequest<string> // کد رهگیری را برمی‌گرداند
    {
        public int OrderId { get; set; }
    }

    /// <summary>
    /// هندلر برای کامند ثبت مرسوله.
    /// </summary>
    public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogisticsService _logisticsService;

        public CreateShipmentCommandHandler(IUnitOfWork unitOfWork, ILogisticsService logisticsService)
        {
            _unitOfWork = unitOfWork;
            _logisticsService = logisticsService;
        }

        public async Task<string> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            // ۱. سفارش را به همراه آیتم‌ها و آدرس از دیتابیس می‌خوانیم
            var order = await _unitOfWork.OrderRepository.GetOrderWithDetailsAsync(request.OrderId);
            if (order == null)
            {
                throw new NotFoundException("سفارش یافت نشد.");
            }

            if (string.IsNullOrEmpty(order.ShippingMethod))
            {
                throw new BadRequestException("روش ارسال برای این سفارش مشخص نشده است.");
            }

            // ۲. جزئیات بسته را دوباره می‌سازیم (وزن، ابعاد، آدرس)
            double totalWeightInGrams = 0;
            // ... (منطق پیچیده‌تر برای ابعاد می‌تواند اضافه شود)
            foreach (var item in order.OrderItems)
            {
                // برای این کار نیاز داریم که OrderItem به واریانت دسترسی داشته باشد
                var variant = await _unitOfWork.ProductVariantRepository.GetByIdAsync(item.ProductVariantId);
                if (variant != null)
                {
                    totalWeightInGrams += variant.Weight * item.Quantity;
                }
            }

            var shipmentDetails = new ShipmentDetailsDto
            {
                WeightInKg = totalWeightInGrams / 1000.0,
                DestinationAddress = new AddressDto
                {
                    Province = order.Address.Province,
                    City = order.Address.City,
                    PostalCode = order.Address.PostalCode,
                    FullAddress = order.Address.AddressLine
                },
                // ابعاد فرضی
                LengthInCm = 30,
                WidthInCm = 20,
                HeightInCm = 15
            };

            // ۳. نام سرویس‌دهنده را از متد ارسال استخراج می‌کنیم (مثلا "post" از "post-پیشتاز")
            var providerName = order.ShippingMethod.Split('-').First();

            // ۴. با سرویس لجستیک تماس گرفته و مرسوله را ثبت می‌کنیم
            var result = await _logisticsService.CreateShipment(providerName, shipmentDetails);

            if (!result.IsSuccess)
            {
                throw new Exception($"خطا در ثبت مرسوله: {result.ErrorMessage}");
            }

            // ۵. کد رهگیری را در سفارش ذخیره کرده و وضعیت را تغییر می‌دهیم
            order.TrackingNumber = result.TrackingNumber;
            order.OrderStatus = "Shipped"; // وضعیت به "ارسال شده" تغییر می‌کند

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CompleteAsync();

            return result.TrackingNumber;
        }
    }
}
