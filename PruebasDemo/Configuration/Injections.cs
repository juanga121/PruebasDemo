using System.Reflection;
using MediatR;
using PruebasDemo.Application.Behaviors;
using PruebasDemo.Application.Creditos.Commands.CrearCredito;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Infrastructure.Repositories;

namespace PruebasDemo.Configuration
{
    public static class Injections
    {
        public static IServiceCollection AddRepositoryDependency(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<CrearCreditoCommand>();
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            var assembly = typeof(CrearCreditoCommand).Assembly;
            var voidRequestTypes = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false })
                .Where(t => typeof(IRequest).IsAssignableFrom(t));

            foreach (var requestType in voidRequestTypes)
            {
                var behaviorType = typeof(ValidationBehavior<>).MakeGenericType(requestType);
                var serviceType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(Unit));
                services.AddTransient(serviceType, behaviorType);
            }

            return services;
        }
    }
}
