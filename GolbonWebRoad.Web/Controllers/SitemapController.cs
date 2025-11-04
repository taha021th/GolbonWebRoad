using GolbonWebRoad.Web.Services.Seo;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;

namespace GolbonWebRoad.Web.Controllers
{
    // این کنترلر خروجی sitemap.xml را در مسیر /sitemap.xml تولید می‌کند.
    // فایل سایت‌مپ فهرستی از URLهای مهم سایت (خانه/لیست/محصول/...) را برای کرال بهتر ارائه می‌دهد.
    public class SitemapController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ISeoSettingsService _seo;
        public SitemapController(IMediator mediator, ISeoSettingsService seo)
        {
            _mediator = mediator; _seo = seo;
        }

        [HttpGet]
        [Route("sitemap.xml")]
        public async Task<IActionResult> Sitemap()
        {
            var settings = await _seo.GetAsync();
            if (!settings.SitemapEnabled)
                return NotFound();

            var req = HttpContext.Request;
            var baseUrl = $"{req.Scheme}://{req.Host}";

            // دریافت داده‌های موردنیاز برای ساخت URLها
            var products = await _mediator.Send(new GolbonWebRoad.Application.Features.Products.Queries.GetProductsQuery { JoinImages = false, JoinCategory = false, JoinBrand = false, JoinReviews = false });
            var categories = await _mediator.Send(new GolbonWebRoad.Application.Features.Categories.Queries.GetCategoriesQuery());
            var brands = await _mediator.Send(new GolbonWebRoad.Application.Features.Brands.Queries.GetBrandsQuery());

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var urlset = new XElement(ns + "urlset");

            // تابع کمکی برای اضافه کردن هر URL به سایت‌مپ
            void AddUrl(string loc, DateTime? lastmod = null, string changefreq = "weekly", string priority = "0.8")
            {
                urlset.Add(new XElement(ns + "url",
                    new XElement(ns + "loc", loc),
                    lastmod.HasValue ? new XElement(ns + "lastmod", lastmod.Value.ToString("yyyy-MM-dd")) : null,
                    new XElement(ns + "changefreq", changefreq),
                    new XElement(ns + "priority", priority)
                ));
            }

            // صفحه اصلی
            AddUrl(baseUrl + "/");

            // لیست محصولات
            AddUrl(baseUrl + "/Products/Index", changefreq: "daily", priority: "0.6");

            // صفحات فیلتر دسته و برند
            foreach (var c in categories)
            {
                AddUrl(baseUrl + $"/Products/Index?categoryId={c.Id}", priority: "0.5");
            }
            foreach (var b in brands)
            {
                AddUrl(baseUrl + $"/Products/Index?brandId={b.Id}", priority: "0.5");
            }
            // جزئیات هر محصول
            foreach (var p in products)
            {
                AddUrl(baseUrl + $"/Products/Detail/{p.Id}", lastmod: p.CreatedAt, changefreq: "weekly", priority: "0.7");
            }

            var doc = new XDocument(urlset);
            return Content(doc.ToString(SaveOptions.DisableFormatting), "application/xml", Encoding.UTF8);
        }
    }
}
