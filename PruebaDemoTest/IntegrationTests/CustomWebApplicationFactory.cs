using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PruebasDemo.Constants;
using PruebasDemo.Infrastructure.Data;
using System.Linq;

namespace PruebaDemoTest.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(ApiConstants.TestingEnv);
        }
    }
}
