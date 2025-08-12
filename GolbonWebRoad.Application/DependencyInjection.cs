using GolbonWebRoad.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GolbonWebRoad.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // این خط با وجود using بالا، دیگر خطا نخواهد داد
            services.AddAutoMapper(config =>
            {
                config.AddProfile<MappingProfile>();
            }
            );

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}