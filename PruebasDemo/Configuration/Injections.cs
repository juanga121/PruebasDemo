using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Interfaces.Services;
using PruebasDemo.Application.Services;
using PruebasDemo.Infrastructure.Repositories;

namespace PruebasDemo.Configuration
{
    public static class Injections
    {
        public static IServiceCollection AddRepositoryDependency(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<CreditosService>();
            services.AddScoped<ICreditoService, CreditosService>();
            return services;
        }
    }
}
