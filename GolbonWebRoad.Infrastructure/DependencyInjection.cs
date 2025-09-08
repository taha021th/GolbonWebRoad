using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Infrastructure.Persistence;
using GolbonWebRoad.Infrastructure.Repositories;
using GolbonWebRoad.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace GolbonWebRoad.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString("GolbonWebRoadShopDbConnection");
            services.AddDbContext<GolbonWebRoadDbContext>(options =>
                options.UseNpgsql(connectionString)
            );
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

            })
            .AddEntityFrameworkStores<GolbonWebRoadDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();


            // اضافه کردن سرویس‌های Identity
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IColorRepository, ColorRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ILogRepository, LogRepository>();
            return services;

        }
    }
}
