using MediatR;
using Microsoft.Extensions.Logging;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Application.Resources.Constants;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Application.Credits.Commands.PayInstallment;

public class PayInstallmentCommandHandler(
    IGenericRepository<Credit, Guid> repository,
    ILogger<PayInstallmentCommandHandler> logger) : IRequestHandler<PayInstallmentCommand>
{
    public async Task Handle(PayInstallmentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var credit = await repository.FindByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException(Messages.CreditNotFound);

        ApplyPayment(credit, dto.PaymentAmount);

        logger.LogInformation(LogTemplates.PaymentMade, credit.Id, dto.PaymentAmount);
        await repository.UpdateAsync(credit);
    }

    private static void ApplyPayment(Credit credit, decimal paymentAmount)
    {
        credit.Balance -= paymentAmount;

        if (credit.Balance == 0)
            credit.Status = Domain.Enums.CreditStatus.Paid;
    }
}
