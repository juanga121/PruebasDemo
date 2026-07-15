using MediatR;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;

namespace PruebasDemo.Application.Credits.Commands.DeleteCredit;

public class DeleteCreditCommandHandler(
    IGenericRepository<Domain.Entities.Credit, Guid> repository)
    : IRequestHandler<DeleteCreditCommand>
{
    public async Task Handle(DeleteCreditCommand request, CancellationToken cancellationToken)
    {
        var existingCredit = await repository.FindByIdAsync(request.Id)
            ?? throw new KeyNotFoundException(Messages.CreditNotFound);

        await repository.DeleteAsync(existingCredit.Id);
    }
}
