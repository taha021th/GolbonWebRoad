namespace GolbonWebRoad.Application.Dtos.Logistics
{
    /// <summary>
    /// این کلاس یک DTO (Data Transfer Object) است.
    /// وظیفه آن نگهداری و جابجایی تمام اطلاعات لازم درباره یک بسته ارسالی بین لایه‌های مختلف برنامه است.
    /// مثلا از کنترلر به سرویس لجستیک.
    /// </summary>
    public class ShipmentDetailsDto
    {
        /// <summary>
        /// آدرس مقصد که بسته باید به آنجا ارسال شود.
        /// آدرس مبدا معمولا ثابت است و از تنظیمات خوانده می‌شود.
        /// </summary>
        public AddressDto DestinationAddress { get; set; }

        /// <summary>
        /// وزن کل بسته به کیلوگرم.
        /// </summary>
        public double WeightInKg { get; set; }

        /// <summary>
        /// طول بسته به سانتی‌متر.
        /// </summary>
        public double LengthInCm { get; set; }

        /// <summary>
        /// عرض بسته به سانتی‌متر.
        /// </summary>
        public double WidthInCm { get; set; }

        /// <summary>
        /// ارتفاع بسته به سانتی‌متر.
        /// </summary>
        public double HeightInCm { get; set; }
    }

    /// <summary>
    /// یک نمایش ساده از آدرس برای استفاده در محاسبات هزینه ارسال.
    /// </summary>
    public class AddressDto
    {
        /// <summary>
        /// استان
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// شهر
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// کد پستی
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// آدرس کامل
        /// </summary>
        public string FullAddress { get; set; }
    }
}

