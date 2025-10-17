using GolbonWebRoad.Application.Dtos.Logistics;
using GolbonWebRoad.Application.Interfaces.Services.Logistics;

namespace GolbonWebRoad.Infrastructure.Services.Logistics
{
    /// <summary>
    /// این کلاس، پیاده‌سازی اصلی سرویس لجستیک است.
    /// این سرویس مانند یک ارکستراتور یا هماهنگ‌کننده عمل می‌کند.
    /// وظیفه آن این است که از تمام سرویس‌دهنده‌های حمل‌ونقل (آداپتورها) استفاده کند
    /// تا عملیات مورد نیاز را انجام دهد.
    /// </summary>
    public class LogisticsService : ILogisticsService
    {
        // این یک لیست از تمام کلاس‌هایی است که اینترفیس IShippingProvider را پیاده‌سازی کرده‌اند.
        // به لطف Dependency Injection، فریم‌ورک به صورت خودکار تمام آداپتورهای ما (تیپاکس، پست و...)
        // را پیدا کرده و به این کلاس تزریق می‌کند. این قلب الگوی Strategy است.
        private readonly IEnumerable<IShippingProvider> _shippingProviders;

        public LogisticsService(IEnumerable<IShippingProvider> shippingProviders)
        {
            _shippingProviders = shippingProviders;
        }

        /// <summary>
        /// این متد از تمام سرویس‌دهنده‌ها می‌خواهد که قیمت‌های خود را برای یک بسته مشخص اعلام کنند.
        /// </summary>
        public async Task<List<ShippingQuoteDto>> GetShippingQuotes(ShipmentDetailsDto shipmentDetails)
        {
            var allQuotes = new List<ShippingQuoteDto>();

            // ما روی تمام آداپتورهای موجود حلقه می‌زنیم
            foreach (var provider in _shippingProviders)
            {
                try
                {
                    // و از هر کدام جداگانه استعلام قیمت می‌گیریم
                    var quotes = await provider.GetQuotes(shipmentDetails);
                    allQuotes.AddRange(quotes);
                }
                catch (Exception ex)
                {
                    // اگر یکی از سرویس‌ها به خطا خورد، ما از آن صرف نظر کرده و به کار ادامه می‌دهیم
                    // در اینجا می‌توان لاگ هم ثبت کرد
                    Console.WriteLine($"Error getting quotes from {provider.Name}: {ex.Message}");
                }
            }

            // در نهایت، لیست تجمیع شده از تمام گزینه‌های ممکن را برمی‌گردانیم
            return allQuotes;
        }

        /// <summary>
        /// این متد، سرویس‌دهنده مورد نظر را پیدا کرده و وظیفه ثبت مرسوله را به آن محول می‌کند.
        /// </summary>
        /// <param name="providerName">نام سرویس‌دهنده‌ای که کاربر انتخاب کرده (مثلا "tipax")</param>
        public async Task<ShippingResultDto> CreateShipment(string providerName, ShipmentDetailsDto shipmentDetails)
        {
            // از بین تمام آداپتورها، آنی را که نامش با نام انتخابی کاربر یکی است، پیدا می‌کنیم
            var provider = _shippingProviders.FirstOrDefault(p => p.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
            {
                // اگر سرویس‌دهنده پیدا نشد، خطا برمی‌گردانیم
                return new ShippingResultDto { IsSuccess = false, ErrorMessage = "سرویس ارسال معتبر نیست." };
            }

            // در غیر این صورت، وظیفه ثبت سفارش را به همان آداپتور می‌سپاریم
            return await provider.CreateShipment(shipmentDetails);
        }
    }
}
