namespace GolbonWebRoad.Web.Services.Seo
{
    // این مدل تنظیمات کلی سئو سایت را نگهداری می‌کند (robots.txt، sitemap و قالب‌های متا)
    public class SeoSettings
    {
        // محتوای فایل robots.txt برای کنترل دسترسی خزنده‌ها (Googlebot و ...)
        public string? RobotsTxt { get; set; }
        // فعال‌/غیرفعال کردن تولید خودکار فایل sitemap.xml در مسیر /sitemap.xml
        public bool SitemapEnabled { get; set; } = true;
        // قالب پیش‌فرض عنوان صفحات وقتی مقدار اختصاصی تنظیم نشده (از {name} برای جایگزینی نام استفاده کنید)
        public string? DefaultMetaTitleTemplate { get; set; } = "{name} | فروشگاه";
        // قالب پیش‌فرض توضیحات متا وقتی مقدار اختصاصی تنظیم نشده
        public string? DefaultMetaDescriptionTemplate { get; set; } = "خرید {name} با بهترین قیمت";
        // فعال‌سازی تولید خودکار تگ canonical برای جلوگیری از محتوای تکراری
        public bool AutoCanonicalEnabled { get; set; } = true;
        // اگر لیست محصولات فیلتر شده باشد (برند/دسته/جستجو) تگ meta robots=noindex,follow اعمال شود
        public bool NoindexOnFilteredLists { get; set; } = true;
        // زمان آخرین بروزرسانی تنظیمات
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    // قرارداد سرویس تنظیمات سئو (دریافت/ذخیره تنظیمات و تولید آدرس مطلق سایت‌مپ)
    public interface ISeoSettingsService
    {
        // دریافت تنظیمات فعلی (در صورت نبودن فایل، مقادیر پیش‌فرض برگردانده می‌شود)
        Task<SeoSettings> GetAsync(CancellationToken ct = default);
        // ذخیره تنظیمات روی دیسک/ذخیره‌ساز
        Task SaveAsync(SeoSettings settings, CancellationToken ct = default);
        // تولید آدرس کامل sitemap.xml متناسب با دامنه جاری درخواست
        string GetSitemapAbsoluteUrl(HttpContext httpContext);
    }
}
