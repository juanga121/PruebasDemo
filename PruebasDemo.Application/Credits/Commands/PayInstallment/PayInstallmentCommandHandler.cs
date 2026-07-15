using MediatR;
using Microsoft.Extensions.Logging;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Application.Resources.Constants;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Application.Credits.Commands.PayInstallment;

public class PayInstallmentCommandHandler(
    IGenericRepository<Credit, Guid> repository,
    ILogger<PayInstallmentCommandHandler> logger) : IRequestHandler<PayInstallmentCommand>
{
    public async Task Handle(PayInstallmentCommand request, CancellationToken cancellationToken)
    {
        PayInstallmentDto paymentRequest = request.Dto;

        Credit? credit = await repository.FindByIdAsync(paymentRequest.Id)
            ?? throw new KeyNotFoundException(Messages.CreditNotFound);

        ApplyPayment(credit, paymentRequest.PaymentAmount);

        logger.LogInformation(LogTemplates.PaymentMade, credit.Id, paymentRequest.PaymentAmount);
        await repository.UpdateAsync(credit);
    }

    private static void ApplyPayment(Credit credit, decimal paymentAmount)
    {
        credit.Balance -= paymentAmount;

        if (credit.Balance == 0)
            credit.Status = Domain.Enums.CreditStatus.Paid;
    }
}
