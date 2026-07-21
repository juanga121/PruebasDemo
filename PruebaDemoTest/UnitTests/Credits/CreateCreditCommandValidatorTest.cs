using FluentValidation.TestHelper;
using PruebasDemo.Application.Credits.Commands.CreateCredit;
using PruebasDemo.Application.Resources;
using PruebaDemoTest.Seeds;

namespace PruebaDemoTest.UnitTests.Credits;

public class CreateCreditCommandValidatorTest
{
    private readonly CreateCreditCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_Amount_Is_Zero()
    {
        var command = new CreateCreditCommand(CreditSeeds.WithZeroAmount);
        TestValidationResult<CreateCreditCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Credit.Amount)
              .WithErrorMessage(Messages.AmountMustBePositive);
    }

    [Fact]
    public void Should_HaveError_When_InterestRate_Is_Negative()
    {
        var command = new CreateCreditCommand(CreditSeeds.WithNegativeRate);
        TestValidationResult<CreateCreditCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Credit.InterestRate)
              .WithErrorMessage(Messages.RateMustBePositive);
    }

    [Fact]
    public void Should_HaveError_When_Months_Is_Zero()
    {
        var command = new CreateCreditCommand(CreditSeeds.WithZeroMonths);
        TestValidationResult<CreateCreditCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Credit.Months)
              .WithErrorMessage(Messages.MonthMustBePositive);
    }

    [Fact]
    public void Should_NotHaveErrors_When_Model_Is_Valid()
    {
        var command = new CreateCreditCommand(CreditSeeds.CreateCreditDto);
        TestValidationResult<CreateCreditCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
