using GolbonWebRoad.Application.Dtos.Logistics;

namespace GolbonWebRoad.Application.Interfaces.Services.Logistics
{
    /// <summary>
    /// این اینترفیس، قرارداد اصلی برای سرویس هماهنگ‌کننده لجستیک است.
    /// هر کلاسی که این اینترفیس را پیاده‌سازی کند، باید بتواند کارهای اصلی لجستیک را انجام دهد.
    /// این کار به ما کمک می‌کند که در لایه Web (کنترلرها)، به جای وابستگی به یک کلاس خاص،
    /// به این اینترفیس وابسته باشیم که باعث تمیزی و انعطاف‌پذیری کد می‌شود.
    /// </summary>
    public interface ILogisticsService
    {
        /// <summary>
        /// تمام گزینه‌های ارسال ممکن و هزینه‌های آن‌ها را برای یک مرسوله مشخص برمی‌گرداند.
        /// این متد در صفحه تسویه حساب (Checkout) استفاده خواهد شد.
        /// </summary>
        Task<List<ShippingQuoteDto>> GetShippingQuotes(ShipmentDetailsDto shipmentDetails);

        /// <summary>
        /// مرسوله را در سیستم شرکت پستی انتخاب شده ثبت نهایی کرده و نتیجه را برمی‌گرداند.
        /// این متد در پنل ادمین استفاده خواهد شد.
        /// </summary>
        Task<ShippingResultDto> CreateShipment(string providerName, ShipmentDetailsDto shipmentDetails);
    }
}

