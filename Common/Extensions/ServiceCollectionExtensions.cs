using TWeb.Services;
using TWeb.Services.Interfaces;

namespace TWeb.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<ICarService, CarService>();
            
            return services;
        }
    }
}
