using Microsoft.EntityFrameworkCore;
using PruebasDemo.Infrastructure.Data;

namespace PruebasDemo.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            var connectionString = configuration.GetConnectionString("ConexionDB");

            if (environment.IsEnvironment("Testing"))
            {
                services.AddDbContext<DataContext>(options =>
                    options.UseInMemoryDatabase("PruebasDemo_TestingDb"));
            }
            else
            {
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(connectionString));
            }

            return services;
        }
    }
}
