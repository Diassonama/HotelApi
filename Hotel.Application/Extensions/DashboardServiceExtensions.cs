using Hotel.Application.Interfaces;
using Hotel.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.Application.Extensions
{
    public static class DashboardServiceExtensions
    {
        /// <summary>
        /// Registra os serviços relacionados ao Dashboard no container de DI
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDashboardServices(this IServiceCollection services)
        {
          //  services.AddScoped<IDashboardService, DashboardService>();
            
            return services;
        }
    }
}
