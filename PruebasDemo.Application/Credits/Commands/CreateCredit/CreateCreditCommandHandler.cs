using MediatR;
using Microsoft.Extensions.Logging;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources.Constants;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;

namespace PruebasDemo.Application.Credits.Commands.CreateCredit;

public class CreateCreditCommandHandler(
    IGenericRepository<Credit, Guid> repository,
    ILogger<CreateCreditCommandHandler> logger) : IRequestHandler<CreateCreditCommand>
{
    public async Task Handle(CreateCreditCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Credit;

        var credit = new Credit
        {
            Id = Guid.NewGuid(),
            Amount = dto.Amount,
            Balance = dto.Amount,
            InterestRate = dto.InterestRate,
            Months = dto.Months,
            Status = CreditStatus.Active
        };

        logger.LogInformation(LogTemplates.CreditCreate, credit.Id);
        await repository.CreateAsync(credit);
    }
}
