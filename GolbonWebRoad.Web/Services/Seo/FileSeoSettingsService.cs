using System.Text.Json;

namespace GolbonWebRoad.Web.Services.Seo
{
    // پیاده‌سازی ساده سرویس تنظیمات سئو با ذخیره‌سازی فایل JSON در پوشه wwwroot/seo
    public class FileSeoSettingsService : ISeoSettingsService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileSeoSettingsService> _logger;
        private readonly string _settingsPath;

        // در سازنده مسیر فایل تنظیمات را می‌سازیم: wwwroot/seo/settings.json
        public FileSeoSettingsService(IWebHostEnvironment env, ILogger<FileSeoSettingsService> logger)
        {
            _env = env;
            _logger = logger;
            var dir = Path.Combine(_env.WebRootPath ?? "wwwroot", "seo"); // پوشه نگهداری تنظیمات سئو
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            _settingsPath = Path.Combine(dir, "settings.json");
        }

        // خواندن تنظیمات از فایل. اگر فایل وجود نداشته باشد یا مشکلی رخ دهد، تنظیمات پیش‌فرض برگردانده می‌شود
        public async Task<SeoSettings> GetAsync(CancellationToken ct = default)
        {
            try
            {
                if (!File.Exists(_settingsPath))
                {
                    // اولین اجرا: فایل نیست، مقدار پیش‌فرض robots.txt برمی‌گردد
                    return new SeoSettings
                    {
                        RobotsTxt = DefaultRobots()
                    };
                }
                using var fs = File.OpenRead(_settingsPath);
                var settings = await JsonSerializer.DeserializeAsync<SeoSettings>(fs, cancellationToken: ct) ?? new SeoSettings();
                if (string.IsNullOrWhiteSpace(settings.RobotsTxt)) settings.RobotsTxt = DefaultRobots();
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read SEO settings. Using defaults.");
                return new SeoSettings { RobotsTxt = DefaultRobots() };
            }
        }

        // ذخیره تنظیمات در فایل JSON (با فرمت خوانا)
        public async Task SaveAsync(SeoSettings settings, CancellationToken ct = default)
        {
            try
            {
                settings.LastUpdated = DateTime.UtcNow;
                await using var fs = File.Create(_settingsPath);
                await JsonSerializer.SerializeAsync(fs, settings, new JsonSerializerOptions { WriteIndented = true }, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save SEO settings");
                throw;
            }
        }

        // ساخت آدرس مطلق برای /sitemap.xml با توجه به دامنه درخواست فعلی
        public string GetSitemapAbsoluteUrl(HttpContext httpContext)
        {
            var req = httpContext.Request;
            var baseUrl = $"{req.Scheme}://{req.Host}";
            return baseUrl + "/sitemap.xml";
        }

        // robots.txt پیش‌فرض: اجازه دسترسی به تمام صفحات
        private static string DefaultRobots() => "User-agent: *\nAllow: /\n";
    }
}
