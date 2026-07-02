using MediatR;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;

namespace PruebasDemo.Application.Creditos.Commands.ActualizarCredito;

public class ActualizarCreditoCommandHandler(
    IGenericRepository<Domain.Entities.CreditoEntity, Guid> repository)
    : IRequestHandler<ActualizarCreditoCommand>
{
    public async Task Handle(ActualizarCreditoCommand request, CancellationToken cancellationToken)
    {
        var creditoExistente = await repository.FindByIdAsync(request.Id)
            ?? throw new KeyNotFoundException(Mensajes.CreditoNotFound);

        creditoExistente.Monto = request.Credito.Monto;
        creditoExistente.TasaInteres = request.Credito.TasaInteres;
        creditoExistente.Meses = request.Credito.Meses;

        await repository.UpdateAsync(creditoExistente);
    }
}
