using GolbonWebRoad.Application.Dtos.Logistics;

namespace GolbonWebRoad.Application.Interfaces.Services.Logistics
{
    /// <summary>
    /// این اینترفیس، قلب الگوی طراحی Strategy در سیستم ماست.
    /// این یک "قرارداد" است که هر شرکت حمل‌ونقلی (تیپاکس، پست، و...) باید آن را امضا (پیاده‌سازی) کند.
    /// با این کار، سرویس اصلی لجستیک ما می‌تواند با همه این شرکت‌ها به یک زبان مشترک صحبت کند،
    /// بدون اینکه از جزئیات داخلی آن‌ها خبر داشته باشد.
    /// </summary>
    public interface IShippingProvider
    {
        /// <summary>
        /// یک نام منحصر به فرد برای هر شرکت، مثلا "tipax" یا "post".
        /// این نام به عنوان شناسه در کل سیستم استفاده می‌شود.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// هزینه ارسال را محاسبه کرده و یک یا چند گزینه (Quote) را برمی‌گرداند.
        /// </summary>
        Task<List<ShippingQuoteDto>> GetQuotes(ShipmentDetailsDto shipmentDetails);

        /// <summary>
        /// سفارش را در سیستم شرکت پستی نهایی کرده و شماره رهگیری دریافت می‌کند.
        /// </summary>
        Task<ShippingResultDto> CreateShipment(ShipmentDetailsDto shipmentDetails);
    }
}