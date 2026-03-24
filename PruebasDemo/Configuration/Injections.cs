using PruebasDemo.Application.Repositories;
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
            return services;
        }
    }
}
