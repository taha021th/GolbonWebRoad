using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Application.Features.Dashboard.Queries
{
    /// <summary>
    /// هندلر برای پردازش کوئری دریافت آمار داشبورد
    /// این کلاس مسئول جمع‌آوری آمار از دیتابیس و تولید گزارش کامل داشبورد است
    /// </summary>
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// سازنده هندلر
        /// </summary>
        /// <param name="unitOfWork">واحد کار برای دسترسی به ریپازیتوری‌ها</param>
        /// <param name="userManager">مدیر کاربران برای دریافت آمار کاربران</param>
        public GetDashboardStatsQueryHandler(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        /// <summary>
        /// پردازش کوئری و جمع‌آوری آمار داشبورد از دیتابیس
        /// </summary>
        /// <param name="request">کوئری دریافت آمار</param>
        /// <param name="cancellationToken">توکن لغو عملیات</param>
        /// <returns>آمار کامل داشبورد</returns>
        public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            // استفاده از Repository ها برای دریافت آمار به جای استفاده مستقیم از DbContext

            // محاسبه آمار کلی سیستم از طریق Repository ها
            var totalRevenue = await _unitOfWork.OrderRepository.GetTotalRevenueAsync();
            var totalOrders = await _unitOfWork.OrderRepository.GetTotalOrdersCountAsync();
            var totalUsers = await _userManager.Users.CountAsync(cancellationToken);
            var totalProducts = await _unitOfWork.ProductRepository.GetTotalProductsCountAsync();

            // محاسبه آمار امروز
            var todayRevenue = await _unitOfWork.OrderRepository.GetTodayRevenueAsync();
            var todayOrders = await _unitOfWork.OrderRepository.GetTodayOrdersCountAsync();

            // تعداد سفارشات در انتظار
            var pendingOrders = await _unitOfWork.OrderRepository.GetPendingOrdersCountAsync();

            // تعداد محصولات کم موجود
            var lowStockProducts = await _unitOfWork.ProductRepository.GetLowStockProductsCountAsync();

            // دریافت آخرین سفارشات (۵ مورد اخیر)
            var recentOrdersEntities = await _unitOfWork.OrderRepository.GetRecentOrdersAsync(5);

            // دریافت آمار فروش ۷ روز اخیر
            var dailySalesStats = await _unitOfWork.OrderRepository.GetDailySalesStatsAsync(7);


            var recentOrders = recentOrdersEntities.Select(o => new RecentOrderDto
            {
                Id = o.Id,
                CustomerName = $"{o.User?.FirstName} {o.User?.LastName}".Trim(),
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus,
                OrderDate = o.OrderDate
            }).ToList();


            var dailySales = dailySalesStats.Select(ds => new DailySalesDto
            {
                Date = ds.Date,
                Sales = ds.Sales,
                OrdersCount = ds.OrdersCount
            }).ToList();

            return new DashboardStatsDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TodayRevenue = todayRevenue,
                TodayOrders = todayOrders,
                PendingOrders = pendingOrders,
                LowStockProducts = lowStockProducts,
                RecentOrders = recentOrders,
                DailySales = dailySales
            };
        }
    }
}