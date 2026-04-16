using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Enums;
using PruebasDemo.Infrastructure.Data;
using System.Net.Http.Json;

namespace PruebaDemoTest.PruebasIntegracion.Credito
{
    public class CreditosIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public CreditosIntegrationTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CrearCredito_Endpoint_Post_CreaCreditoEnBD()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var dto = new CreditoDto
            {
                Monto = 100,
                TasaInteres = 10,
                Meses = 12
            };

            var response = await _client.PostAsJsonAsync("/api/credito", dto);

            response.EnsureSuccessStatusCode();

            var credito = await db.Creditos.FirstOrDefaultAsync(c =>
                c.Monto == dto.Monto &&
                c.TasaInteres == dto.TasaInteres &&
                c.Meses == dto.Meses
            );

            Assert.NotNull(credito);
            Assert.Equal(dto.Monto, credito!.Monto);
            Assert.Equal(dto.Monto, credito.Saldo);
            Assert.Equal(CreditoEstado.Activo, credito.Estado);
        }
    }
}
