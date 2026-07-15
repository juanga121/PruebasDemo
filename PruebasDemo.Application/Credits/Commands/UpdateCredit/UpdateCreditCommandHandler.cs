using MediatR;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;

namespace PruebasDemo.Application.Credits.Commands.UpdateCredit;

public class UpdateCreditCommandHandler(
    IGenericRepository<Domain.Entities.Credit, Guid> repository)
    : IRequestHandler<UpdateCreditCommand>
{
    public async Task Handle(UpdateCreditCommand request, CancellationToken cancellationToken)
    {
        var existingCredit = await repository.FindByIdAsync(request.Id)
            ?? throw new KeyNotFoundException(Messages.CreditNotFound);

        existingCredit.Amount = request.Credit.Amount;
        existingCredit.InterestRate = request.Credit.InterestRate;
        existingCredit.Months = request.Credit.Months;

        await repository.UpdateAsync(existingCredit);
    }
}
