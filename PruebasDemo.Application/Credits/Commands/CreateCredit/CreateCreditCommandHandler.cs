using MediatR;
using Microsoft.Extensions.Logging;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources.Constants;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;

namespace PruebasDemo.Application.Credits.Commands.CreateCredit;

public class CreateCreditCommandHandler(
    IGenericRepository<Credit, Guid> repository,
    ILogger<CreateCreditCommandHandler> logger) : IRequestHandler<CreateCreditCommand>
{
    public async Task Handle(CreateCreditCommand request, CancellationToken cancellationToken)
    {
        CreditDto creditRequest = request.Credit;

        Credit credit = new Credit
        {
            Id = Guid.NewGuid(),
            Amount = creditRequest.Amount,
            Balance = creditRequest.Amount,
            InterestRate = creditRequest.InterestRate,
            Months = creditRequest.Months,
            Status = CreditStatus.Active
        };

        logger.LogInformation(LogTemplates.CreditCreate, credit.Id);
        await repository.CreateAsync(credit);
    }
}
