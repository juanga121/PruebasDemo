using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;
using PruebasDemo.Infrastructure.Data;
using PruebaDemoTest.Constants;
using PruebaDemoTest.IntegrationTests;
using PruebaDemoTest.Seeds;
using System.Net.Http.Json;

namespace PruebaDemoTest.IntegrationTests.Credits;

public class CreditsIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CreditsIntegrationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private void ResetDatabase()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();
        db.ResetDatabase();
    }

    private Guid SeedCredit(PruebasDemo.Domain.Entities.Credit credit)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();
        db.ResetDatabase();
        db.SeedCredit(credit);
        return credit.Id;
    }

    [Fact]
    public async Task CreateCredit_Endpoint_Post_CreatesCreditInDb()
    {
        ResetDatabase();

        CreditDto createRequest = Seeded.CreateCreditDto;
        HttpResponseMessage response = await _client.PostAsJsonAsync(ApiRoutes.Credit, createRequest);

        response.EnsureSuccessStatusCode();

        using IServiceScope scope = _factory.Services.CreateScope();
        DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();
        Credit? credit = await db.Credits.FirstOrDefaultAsync(credit =>
            credit.Amount == createRequest.Amount &&
            credit.InterestRate == createRequest.InterestRate &&
            credit.Months == createRequest.Months
        );

        Assert.NotNull(credit);
        Assert.Equal(createRequest.Amount, credit!.Amount);
        Assert.Equal(createRequest.Amount, credit.Balance);
        Assert.Equal(CreditStatus.Active, credit.Status);
    }

    [Fact]
    public async Task GetCredits_Endpoint_Get_ReturnsList()
    {
        SeedCredit(Seeded.ActiveCredit);

        HttpResponseMessage response = await _client.GetAsync(ApiRoutes.Credit);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        Assert.Contains("exito", content);
        Assert.Contains("data", content);
    }

    [Fact]
    public async Task GetCreditById_Endpoint_Get_ReturnsCredit()
    {
        var id = SeedCredit(Seeded.CustomCredit);

        HttpResponseMessage response = await _client.GetAsync(ApiRoutes.CreditById(id));
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        Assert.Contains("exito", content);
        Assert.Contains("data", content);
    }

    [Fact]
    public async Task UpdateCredit_Endpoint_Put_UpdatesCredit()
    {
        var id = SeedCredit(Seeded.ActiveCredit);

        HttpResponseMessage response = await _client.PutAsJsonAsync(ApiRoutes.CreditById(id), Seeded.CreditUpdateDto);

        response.EnsureSuccessStatusCode();

        using IServiceScope scope = _factory.Services.CreateScope();
        DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();
        Credit? updated = await db.Credits.FindAsync(id);

        Assert.NotNull(updated);
        Assert.Equal(Seeded.CreditUpdateDto.Amount, updated!.Amount);
        Assert.Equal(Seeded.CreditUpdateDto.InterestRate, updated.InterestRate);
    }

    [Fact]
    public async Task DeleteCredit_Endpoint_Delete_DeletesCredit()
    {
        var id = SeedCredit(Seeded.ActiveCredit);

        HttpResponseMessage response = await _client.DeleteAsync(ApiRoutes.CreditById(id));
        response.EnsureSuccessStatusCode();

        using IServiceScope scope = _factory.Services.CreateScope();
        DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();
        Credit? deleted = await db.Credits.FindAsync(id);

        Assert.Null(deleted);
    }

    [Fact]
    public async Task PayInstallment_Endpoint_Put_UpdatesBalance()
    {
        var id = SeedCredit(Seeded.ActiveCredit);

        HttpResponseMessage response = await _client.PutAsJsonAsync(ApiRoutes.PayInstallment,
            new { Id = id, PaymentAmount = 50 });
        response.EnsureSuccessStatusCode();

        using IServiceScope scope = _factory.Services.CreateScope();
        DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();
        Credit? updated = await db.Credits.FindAsync(id);

        Assert.NotNull(updated);
        Assert.Equal(50, updated!.Balance);
    }

    #region ErrorPaths

    [Fact]
    public async Task CreateCredit_InvalidData_Returns400()
    {
        ResetDatabase();

        CreditDto invalidDto = new CreditDto { Amount = 0, InterestRate = -1, Months = 0 };
        HttpResponseMessage response = await _client.PostAsJsonAsync(ApiRoutes.Credit, invalidDto);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        Assert.Contains("success", content);
        Assert.Contains("traceId", content);
    }

    [Fact]
    public async Task PayInstallment_ExceedsBalance_Returns400()
    {
        SeedCredit(Seeded.ActiveCredit);

        HttpResponseMessage response = await _client.PutAsJsonAsync(ApiRoutes.PayInstallment,
            new { Id = Seeded.CreditId, PaymentAmount = 999 });

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion
}
