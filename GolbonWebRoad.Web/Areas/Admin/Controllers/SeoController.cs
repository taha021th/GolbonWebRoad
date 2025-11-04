using GolbonWebRoad.Web.Services.Seo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    // کنترلر مدیریت تنظیمات سئو در پنل ادمین (robots.txt، sitemap و قالب‌های متا)
    public class SeoController : Controller
    {
        private readonly ISeoSettingsService _seoSettingsService;
        public SeoController(ISeoSettingsService seoSettingsService)
        {
            _seoSettingsService = seoSettingsService;
        }

        // صفحه نمایش فرم تنظیمات 
        //محتویات فایل روبوتز رو میگیره و داخل تکست ایریا نمایش میده تا ادمین ببینه و بتونه ویرایش کنه
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = await _seoSettingsService.GetAsync();
            return View(settings);
        }

        // ذخیره تغییرات تنظیمات سئو
        //در واقع تنظیمات روبوتز رو تغییر میده ادمین و اینجا ذخیره میشه
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromForm] SeoSettings model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await _seoSettingsService.SaveAsync(model);
            TempData["SuccessMessage"] = "تنظیمات سئو با موفقیت ذخیره شد";
            return RedirectToAction(nameof(Index));
        }
    }
}
