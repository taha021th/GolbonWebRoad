using AutoMapper;
using GolbonWebRoad.Application.Features.Dashboard.Queries;
using GolbonWebRoad.Web.Areas.Admin.Models.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// کنترلر داشبورد پنل ادمین
    /// مسئول نمایش آمار و اطلاعات کلیدی سیستم
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// سازنده کنترلر داشبورد
        /// </summary>
        /// <param name="mediator">واسط برای ارسال کوئری‌ها و کامندها</param>
        /// <param name="mapper">نگاشت خودکار اشیاء</param>
        public DashboardController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// نمایش صفحه اصلی داشبورد با آمار داینامیک
        /// </summary>
        /// <returns>صفحه داشبورد با آمار واقعی</returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                // دریافت آمار از دیتابیس
                var dashboardStats = await _mediator.Send(new GetDashboardStatsQuery());
                
                // تبدیل DTO به ViewModel
                var viewModel = _mapper.Map<DashboardViewModel>(dashboardStats);
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // لوگ خطا و نمایش پیام مناسب
                TempData["ErrorMessage"] = "خطا در بارگیری آمار داشبورد. لطفاً دوباره تلاش کنید.";
                
                // برگرداندن ViewModel خالی در صورت خطا
                return View(new DashboardViewModel());
            }
        }
    }
}
