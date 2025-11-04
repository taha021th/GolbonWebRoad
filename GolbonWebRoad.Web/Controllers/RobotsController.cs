using GolbonWebRoad.Web.Services.Seo;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace GolbonWebRoad.Web.Controllers
{
    // این کنترلر خروجی robots.txt را در مسیر /robots.txt تولید می‌کند.
    // robots.txt به ربات‌های موتورهای جستجو می‌گوید کدام مسیرها را بخزند یا نخواهند.
    public class RobotsController : Controller
    {
        private readonly ISeoSettingsService _seo;
        public RobotsController(ISeoSettingsService seo)
        {
            _seo = seo;
        }

        [HttpGet]
        [Route("robots.txt")]
        public async Task<IActionResult> Robots()
        {
            var settings = await _seo.GetAsync();
            var robots = settings.RobotsTxt ?? "User-agent: *\nAllow: /\n";
            // اگر Sitemap فعال باشد و خط آن داخل robots وجود نداشته باشد، لینک sitemap.xml اضافه می‌شود
            if (settings.SitemapEnabled)
            {
                var sitemapUrl = _seo.GetSitemapAbsoluteUrl(HttpContext);
                if (!robots.Contains("Sitemap:", StringComparison.OrdinalIgnoreCase))
                {
                    robots += $"\nSitemap: {sitemapUrl}\n";
                }
            }
            return Content(robots, "text/plain", Encoding.UTF8);
        }
    }
}
