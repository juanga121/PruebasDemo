using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;
using PruebasDemo.Infrastructure.Data;
using PruebaDemoTest.Seeds;
using System.Net.Http.Json;

namespace PruebaDemoTest.PruebasIntegracion.Credito;

public class CreditosIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CreditosIntegrationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private void ResetDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        db.ResetDatabase();
    }

    private Guid SeedCredito(CreditoEntity credito)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        db.ResetDatabase();
        db.SeedCredito(credito);
        return credito.Id;
    }

    [Fact]
    public async Task CrearCredito_Endpoint_Post_CreaCreditoEnBD()
    {
        ResetDatabase();

        var dto = Seeded.CrearCredito;
        var response = await _client.PostAsJsonAsync("/api/credito", dto);

        response.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
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

    [Fact]
    public async Task ObtenerCreditos_Endpoint_Get_RetornaLista()
    {
        SeedCredito(Seeded.CreditoActivo);

        var response = await _client.GetAsync("/api/credito");
        response.EnsureSuccessStatusCode();

        var contenido = await response.Content.ReadAsStringAsync();
        Assert.Contains("exito", contenido);
        Assert.Contains("data", contenido);
    }

    [Fact]
    public async Task ObtenerCreditoPorId_Endpoint_Get_RetornaCredito()
    {
        var seed = new CreditoEntity
        {
            Id = Guid.NewGuid(), Monto = 200,
            TasaInteres = 5, Meses = 6,
            Saldo = 200, Estado = CreditoEstado.Activo
        };

        var id = SeedCredito(seed);

        var response = await _client.GetAsync($"/api/credito/{id}");
        response.EnsureSuccessStatusCode();

        var contenido = await response.Content.ReadAsStringAsync();
        Assert.Contains("exito", contenido);
        Assert.Contains("data", contenido);
    }

    [Fact]
    public async Task ActualizarCredito_Endpoint_Put_ActualizaCredito()
    {
        var id = SeedCredito(Seeded.CreditoActivo);

        var dto = new CreditoDto { Monto = 500, TasaInteres = 8, Meses = 10 };
        var response = await _client.PutAsJsonAsync($"/api/credito/{id}", dto);

        response.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        var actualizado = await db.Creditos.FindAsync(id);

        Assert.NotNull(actualizado);
        Assert.Equal(dto.Monto, actualizado!.Monto);
        Assert.Equal(dto.TasaInteres, actualizado.TasaInteres);
    }

    [Fact]
    public async Task EliminarCredito_Endpoint_Delete_EliminaCredito()
    {
        var id = SeedCredito(Seeded.CreditoActivo);

        var response = await _client.DeleteAsync($"/api/credito/{id}");
        response.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        var eliminado = await db.Creditos.FindAsync(id);

        Assert.Null(eliminado);
    }

    [Fact]
    public async Task PagarCuota_Endpoint_Put_ActualizaSaldo()
    {
        var id = SeedCredito(Seeded.CreditoActivo);

        var response = await _client.PutAsJsonAsync($"/api/credito/pagar",
            new { Id = id, MontoPago = 50 });
        response.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        var actualizado = await db.Creditos.FindAsync(id);

        Assert.NotNull(actualizado);
        Assert.Equal(50, actualizado!.Saldo);
    }
}
