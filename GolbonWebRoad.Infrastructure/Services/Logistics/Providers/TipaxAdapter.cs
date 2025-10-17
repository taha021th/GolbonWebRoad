using GolbonWebRoad.Application.Dtos.Logistics;
using GolbonWebRoad.Application.Interfaces.Services.Logistics;

namespace GolbonWebRoad.Infrastructure.Services.Logistics.Providers
{
    /// <summary>
    /// این کلاس، آداپتور مخصوص شرکت تیپاکس است.
    /// وظیفه این کلاس این است که درخواست‌های برنامه ما را به زبانی که API تیپاکس می‌فهمد ترجمه کند.
    /// این کلاس، اینترفیس IShippingProvider را پیاده‌سازی می‌کند و به این ترتیب، به سیستم ما قول می‌دهد
    /// که متدهای لازم برای استعلام قیمت و ثبت سفارش را دارد.
    /// </summary>
    public class TipaxAdapter : IShippingProvider
    {
        // نام منحصر به فرد این سرویس‌دهنده که در کل سیستم با این نام شناخته می‌شود.
        public string Name => "tipax";

        /// <summary>
        /// متد استعلام قیمت برای تیپاکس.
        /// در دنیای واقعی، اینجا باید با API تیپاکس تماس گرفته شود.
        /// اما فعلا برای تست، ما یک قیمت ثابت و هاردکد شده را برمی‌گردانیم.
        /// </summary>
        /// <param name="shipmentDetails">جزئیات بسته ارسالی مثل وزن، ابعاد و آدرس مقصد</param>
        /// <returns>لیستی از گزینه‌های ارسال ممکن با قیمت و زمان تحویل</returns>
        public Task<List<ShippingQuoteDto>> GetQuotes(ShipmentDetailsDto shipmentDetails)
        {
            // شبیه‌سازی یک سرویس ارسال اکسپرس از طرف تیپاکس
            var quote = new ShippingQuoteDto
            {
                ProviderName = Name,
                ServiceName = "اکسپرس",
                Cost = 75000, // قیمت فرضی
                EstimatedDeliveryTime = "۱ تا ۳ روز کاری"
            };

            // نتیجه را در قالب یک لیست برمی‌گردانیم
            var result = new List<ShippingQuoteDto> { quote };
            return Task.FromResult(result);
        }

        /// <summary>
        /// متد ثبت نهایی مرسوله در سیستم تیپاکس.
        /// در اینجا هم در آینده با API تیپاکس تماس می‌گیریم و کد رهگیری واقعی دریافت می‌کنیم.
        /// فعلا یک کد رهگیری فرضی تولید می‌کنیم.
        /// </summary>
        /// <param name="shipmentDetails">جزئیات کامل بسته برای ثبت نهایی</param>
        /// <returns>نتیجه ثبت شامل کد رهگیری</returns>
        public Task<ShippingResultDto> CreateShipment(ShipmentDetailsDto shipmentDetails)
        {
            // شبیه‌سازی نتیجه موفقیت‌آمیز
            var result = new ShippingResultDto
            {
                IsSuccess = true,
                ProviderName = Name,
                // تولید یک کد رهگیری تستی و غیرواقعی
                TrackingNumber = $"TPX-{new Random().Next(100000, 999999)}"
            };

            return Task.FromResult(result);
        }
    }
}
