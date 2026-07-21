using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;

namespace PruebaDemoTest.Seeds;

public static class CreditSeeds
{
    public static readonly Guid CreditId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    public static Credit ActiveCredit => new()
    {
        Id = CreditId, Amount = 100, Balance = 100,
        InterestRate = 10, Months = 12, Status = CreditStatus.Active
    };

    public static Credit CreditWithBalance50 => new()
    {
        Id = PaymentSeeds.PayId, Amount = 50, Balance = 50,
        InterestRate = 10, Months = 12, Status = CreditStatus.Active
    };

    public static CreditDto CreateCreditDto => new()
    {
        Amount = 100, InterestRate = 10, Months = 12
    };

    public static CreditDto UpdateCreditDto => new()
    {
        Amount = 200, InterestRate = 10, Months = 24
    };

    public static Credit CustomCredit => new()
    {
        Id = Guid.NewGuid(), Amount = 200,
        InterestRate = 5, Months = 6,
        Balance = 200, Status = CreditStatus.Active
    };

    public static CreditDto CreditUpdateDto => new()
    {
        Amount = 500, InterestRate = 8, Months = 10
    };

    public static CreditDto WithZeroAmount => new() { Amount = 0 };
    public static CreditDto WithNegativeRate => new() { InterestRate = -1 };
    public static CreditDto WithZeroMonths => new() { Months = 0 };

    public static List<Credit> CreditsList =>
    [
        new() { Id = Guid.NewGuid(), Amount = 100 },
        new() { Id = Guid.NewGuid(), Amount = 200 }
    ];
}
