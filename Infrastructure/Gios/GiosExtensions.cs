using Application.Interfaces;
using Infrastructure.Gios.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Gios
{
    public static class GiosExtensions
    {
        public static IServiceCollection AddGiosServices(this IServiceCollection services)
        {
            services.AddScoped<IGiosService, GiosService>();
            return services;
        }
    }
}