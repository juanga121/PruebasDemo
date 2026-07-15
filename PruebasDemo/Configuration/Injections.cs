using System.Reflection;
using MediatR;
using PruebasDemo.Application.Behaviors;
using PruebasDemo.Application.Credits.Commands.CreateCredit;
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
                cfg.RegisterServicesFromAssemblyContaining<CreateCreditCommand>();
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            Assembly assembly = typeof(CreateCreditCommand).Assembly;
            IEnumerable<Type> unitRequestTypes = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false })
                .Where(t => typeof(IRequest).IsAssignableFrom(t));

            foreach (Type requestType in unitRequestTypes)
            {
                Type behaviorType = typeof(ValidationBehavior<>).MakeGenericType(requestType);
                Type serviceType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(Unit));
                services.AddTransient(serviceType, behaviorType);
            }

            return services;
        }
    }
}
