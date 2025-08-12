using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Interfaces;
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

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

            })
            .AddEntityFrameworkStores<GolbonWebRoadDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

            var connectionString = configuration.GetConnectionString("GolbonWebRoadShopDbConnection");
            services.AddDbContext<GolbonWebRoadDbContext>(options =>
                options.UseNpgsql(connectionString)
            );
            // اضافه کردن سرویس‌های Identity
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            return services;

        }
    }
}
