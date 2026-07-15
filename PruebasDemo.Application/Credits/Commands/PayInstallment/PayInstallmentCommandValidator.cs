using FluentValidation;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;

namespace PruebasDemo.Application.Credits.Commands.PayInstallment;

public class PayInstallmentCommandValidator : AbstractValidator<PayInstallmentCommand>
{
    public PayInstallmentCommandValidator(IGenericRepository<Credit, Guid> repository)
    {
        RuleFor(x => x.Dto.PaymentAmount)
            .GreaterThan(0)
            .WithMessage(Messages.PaymentMustBePositive);

        RuleFor(x => x).CustomAsync(async (command, context, _) =>
        {
            if (command.Dto.PaymentAmount <= 0) return;

            var credit = await repository.FindByIdAsync(command.Dto.Id);

            ValidateCreditExists(credit, context);
            ValidateCreditIsActive(credit, context);
            ValidatePaymentAmount(command.Dto, credit, context);
        });
    }

    private static void ValidateCreditExists(Credit? credit, ValidationContext<PayInstallmentCommand> context)
    {
        if (credit is null)
            context.AddFailure(nameof(PayInstallmentCommand.Dto) + "." + nameof(PayInstallmentDto.Id), Messages.CreditNotFound);
    }

    private static void ValidateCreditIsActive(Credit? credit, ValidationContext<PayInstallmentCommand> context)
    {
        if (credit is not null && credit.Status != CreditStatus.Active)
            context.AddFailure(Messages.CreditNotActive);
    }

    private static void ValidatePaymentAmount(PayInstallmentDto paymentData, Credit? credit, ValidationContext<PayInstallmentCommand> context)
    {
        if (credit is not null && paymentData.PaymentAmount > credit.Balance)
            context.AddFailure(nameof(PayInstallmentCommand.Dto) + "." + nameof(PayInstallmentDto.PaymentAmount), Messages.PaymentExceedsBalance);
    }
}
