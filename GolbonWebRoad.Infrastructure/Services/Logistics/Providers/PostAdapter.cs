using GolbonWebRoad.Application.Dtos.Logistics;
using GolbonWebRoad.Application.Interfaces.Services.Logistics;

namespace GolbonWebRoad.Infrastructure.Services.Logistics.Providers
{
    /// <summary>
    /// این کلاس، آداپتور مخصوص شرکت پست جمهوری اسلامی ایران است.
    /// ساختار این کلاس دقیقا مشابه آداپتور تیپاکس است، اما منطق داخلی آن
    /// در آینده برای کار با وب‌سرویس‌های شرکت پست نوشته خواهد شد.
    /// </summary>
    public class PostAdapter : IShippingProvider
    {
        public string Name => "post";

        /// <summary>
        /// متد استعلام قیمت برای شرکت پست.
        /// فعلا برای تست، دو سرویس "پیشتاز" و "سفارشی" را با قیمت‌های فرضی برمی‌گردانیم.
        /// </summary>
        public Task<List<ShippingQuoteDto>> GetQuotes(ShipmentDetailsDto shipmentDetails)
        {
            var pishtazQuote = new ShippingQuoteDto
            {
                ProviderName = Name,
                ServiceName = "پیشتاز",
                Cost = 45000,
                EstimatedDeliveryTime = "۲ تا ۴ روز کاری"
            };

            var sefareshiQuote = new ShippingQuoteDto
            {
                ProviderName = Name,
                ServiceName = "سفارشی",
                Cost = 35000,
                EstimatedDeliveryTime = "۳ تا ۶ روز کاری"
            };

            var result = new List<ShippingQuoteDto> { pishtazQuote, sefareshiQuote };
            return Task.FromResult(result);
        }

        /// <summary>
        /// متد ثبت نهایی مرسوله در سیستم پست.
        /// </summary>
        public Task<ShippingResultDto> CreateShipment(ShipmentDetailsDto shipmentDetails)
        {
            var result = new ShippingResultDto
            {
                IsSuccess = true,
                ProviderName = Name,
                TrackingNumber = $"POST-{new Random().Next(10000000, 99999999)}"
            };

            return Task.FromResult(result);
        }
    }
}
