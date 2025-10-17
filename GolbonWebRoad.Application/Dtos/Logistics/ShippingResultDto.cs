namespace GolbonWebRoad.Application.Dtos.Logistics
{
    /// <summary>
    /// این کلاس، نتیجه‌ی عملیات ثبت نهایی یک مرسوله در سیستم یک شرکت پستی را نگهداری می‌کند.
    /// </summary>
    public class ShippingResultDto
    {
        /// <summary>
        /// آیا عملیات ثبت موفقیت‌آمیز بوده است؟
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// شماره رهگیری دریافت شده از شرکت پستی.
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// نام شرکتی که مرسوله در آن ثبت شده.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// در صورت بروز خطا، پیام خطا در این فیلد قرار می‌گیرد.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// فایل لیبل پستی (اختیاری). بعضی از APIها فایل PDF یا عکس لیبل را برمی‌گردانند.
        /// </summary>
        public byte[]? ShippingLabel { get; set; }
    }
}

