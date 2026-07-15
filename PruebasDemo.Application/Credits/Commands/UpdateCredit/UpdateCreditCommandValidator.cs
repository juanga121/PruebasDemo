using FluentValidation;
using PruebasDemo.Application.Resources;

namespace PruebasDemo.Application.Credits.Commands.UpdateCredit;

public class UpdateCreditCommandValidator : AbstractValidator<UpdateCreditCommand>
{
    public UpdateCreditCommandValidator()
    {
        RuleFor(x => x.Credit.Amount)
            .GreaterThan(0)
            .WithMessage(Messages.AmountMustBePositive);

        RuleFor(x => x.Credit.InterestRate)
            .GreaterThanOrEqualTo(0)
            .WithMessage(Messages.RateMustBePositive);

        RuleFor(x => x.Credit.Months)
            .GreaterThan(0)
            .WithMessage(Messages.MonthMustBePositive);
    }
}
