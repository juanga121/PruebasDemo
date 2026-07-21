using PruebasDemo.Domain.DTO;

namespace PruebaDemoTest.Seeds;

public static class PaymentSeeds
{
    public static readonly Guid PayId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");

    public static PayInstallmentDto PartialPayment => new()
    {
        Id = CreditSeeds.CreditId, PaymentAmount = 30
    };

    public static PayInstallmentDto ExactPayment => new()
    {
        Id = PayId, PaymentAmount = 50
    };

    public static PayInstallmentDto Payment(decimal amount, Guid id) => new()
    {
        Id = id, PaymentAmount = amount
    };
}
