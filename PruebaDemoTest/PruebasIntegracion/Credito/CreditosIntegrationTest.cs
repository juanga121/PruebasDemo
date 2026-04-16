using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
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
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            var dto = new CreditoDto
            {
                Monto = 100,
                TasaInteres = 10,
                Meses = 12
            };

            var response = await _client.PostAsJsonAsync("/api/credito", dto);

            response.EnsureSuccessStatusCode();

            using var scope2 = _factory.Services.CreateScope();
            var db2 = scope2.ServiceProvider.GetRequiredService<DataContext>();

            var credito = await db2.Creditos.FirstOrDefaultAsync(c =>
                c.Monto == dto.Monto &&
                c.TasaInteres == dto.TasaInteres &&
                c.Meses == dto.Meses
            );

            Assert.NotNull(credito);
            Assert.Equal(dto.Monto, credito!.Monto);
            Assert.Equal(dto.Monto, credito.Saldo);
            Assert.Equal(CreditoEstado.Activo, credito.Estado);
        }

        [Fact]
        public async Task ObtenerCreditos_Endpoint_Get_RetornaLista()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Creditos.Add(new CreditoEntity
                {
                    Id = Guid.NewGuid(),
                    Monto = 100,
                    TasaInteres = 10,
                    Meses = 12,
                    Saldo = 100,
                    Estado = CreditoEstado.Activo
                });

                db.SaveChanges();
            }

            var response = await _client.GetAsync("/api/credito");

            response.EnsureSuccessStatusCode();

            var contenido = await response.Content.ReadAsStringAsync();
            Assert.Contains("exito", contenido);
            Assert.Contains("data", contenido);
        }

        [Fact]
        public async Task ObtenerCreditoPorId_Endpoint_Get_RetornaCredito()
        {
            Guid id;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var credito = new CreditoEntity
                {
                    Id = Guid.NewGuid(),
                    Monto = 200,
                    TasaInteres = 5,
                    Meses = 6,
                    Saldo = 200,
                    Estado = CreditoEstado.Activo
                };

                db.Creditos.Add(credito);
                db.SaveChanges();

                id = credito.Id;
            }

            var response = await _client.GetAsync($"/api/credito/{id}");

            response.EnsureSuccessStatusCode();

            var contenido = await response.Content.ReadAsStringAsync();
            Assert.Contains("exito", contenido);
            Assert.Contains("data", contenido);
        }

        [Fact]
        public async Task ActualizarCredito_Endpoint_Put_ActualizaCredito()
        {
            Guid id;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var credito = new CreditoEntity
                {
                    Id = Guid.NewGuid(),
                    Monto = 100,
                    TasaInteres = 10,
                    Meses = 12,
                    Saldo = 100,
                    Estado = CreditoEstado.Activo
                };

                db.Creditos.Add(credito);
                db.SaveChanges();

                id = credito.Id;
            }

            var dto = new CreditoDto
            {
                Monto = 500,
                TasaInteres = 8,
                Meses = 10
            };

            var response = await _client.PutAsJsonAsync($"/api/credito/{id}", dto);

            response.EnsureSuccessStatusCode();

            using var scope2 = _factory.Services.CreateScope();
            var db2 = scope2.ServiceProvider.GetRequiredService<DataContext>();

            var actualizado = await db2.Creditos.FindAsync(id);

            Assert.NotNull(actualizado);
            Assert.Equal(dto.Monto, actualizado!.Monto);
            Assert.Equal(dto.TasaInteres, actualizado.TasaInteres);
        }

        [Fact]
        public async Task EliminarCredito_Endpoint_Delete_EliminaCredito()
        {
            Guid id;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var credito = new CreditoEntity
                {
                    Id = Guid.NewGuid(),
                    Monto = 100,
                    TasaInteres = 10,
                    Meses = 12,
                    Saldo = 100,
                    Estado = CreditoEstado.Activo
                };

                db.Creditos.Add(credito);
                db.SaveChanges();

                id = credito.Id;
            }

            var response = await _client.DeleteAsync($"/api/credito/{id}");

            response.EnsureSuccessStatusCode();

            using var scope2 = _factory.Services.CreateScope();
            var db2 = scope2.ServiceProvider.GetRequiredService<DataContext>();

            var eliminado = await db2.Creditos.FindAsync(id);

            Assert.Null(eliminado);
        }

        [Fact]
        public async Task PagarCuota_Endpoint_Put_ActualizaSaldo()
        {
            Guid id;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var credito = new CreditoEntity
                {
                    Id = Guid.NewGuid(),
                    Monto = 100,
                    TasaInteres = 10,
                    Meses = 12,
                    Saldo = 100,
                    Estado = CreditoEstado.Activo
                };

                db.Creditos.Add(credito);
                db.SaveChanges();

                id = credito.Id;
            }

            decimal pago = 50;

            var response = await _client.PutAsJsonAsync($"/api/credito/pagar/{id}", pago);

            response.EnsureSuccessStatusCode();

            using var scope2 = _factory.Services.CreateScope();
            var db2 = scope2.ServiceProvider.GetRequiredService<DataContext>();

            var actualizado = await db2.Creditos.FindAsync(id);

            Assert.NotNull(actualizado);
            Assert.Equal(50, actualizado!.Saldo);
        }
    }
}
