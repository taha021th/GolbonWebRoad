namespace GolbonWebRoad.Application.Dtos.Logistics
{
    /// <summary>
    /// این کلاس نمایانگر یک گزینه ارسال (یک استعلام قیمت) است که از یک شرکت حمل‌ونقل دریافت می‌شود.
    /// مثلا "پست پیشتاز با هزینه ۴۵ هزار تومان".
    /// </summary>
    public class ShippingQuoteDto
    {
        /// <summary>
        /// نام شرکت ارائه‌دهنده سرویس (مثلا "post" یا "tipax").
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// نام سرویس خاص آن شرکت (مثلا "پیشتاز" یا "اکسپرس").
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// یک نام کامل و قابل نمایش برای کاربر (مثلا "tipax - اکسپرس").
        /// </summary>
        public string FullServiceName => $"{ProviderName} - {ServiceName}";

        /// <summary>
        /// هزینه این روش ارسال.
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// زمان تخمینی تحویل مرسوله به مشتری (مثلا "۱ تا ۳ روز کاری").
        /// </summary>
        public string EstimatedDeliveryTime { get; set; }
    }
}

