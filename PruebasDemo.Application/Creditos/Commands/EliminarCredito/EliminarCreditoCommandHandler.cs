using MediatR;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;

namespace PruebasDemo.Application.Creditos.Commands.EliminarCredito;

public class EliminarCreditoCommandHandler(
    IGenericRepository<Domain.Entities.CreditoEntity, Guid> repository)
    : IRequestHandler<EliminarCreditoCommand>
{
    public async Task Handle(EliminarCreditoCommand request, CancellationToken cancellationToken)
    {
        var creditoExistente = await repository.FindByIdAsync(request.Id)
            ?? throw new KeyNotFoundException(Mensajes.CreditoNotFound);

        await repository.DeleteAsync(creditoExistente.Id);
    }
}
