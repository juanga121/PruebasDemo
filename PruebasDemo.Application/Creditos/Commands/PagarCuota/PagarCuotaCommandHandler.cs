using MediatR;
using Microsoft.Extensions.Logging;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Application.Resources.Constants;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Application.Creditos.Commands.PagarCuota;

public class PagarCuotaCommandHandler(
    IGenericRepository<CreditoEntity, Guid> repository,
    ILogger<PagarCuotaCommandHandler> logger) : IRequestHandler<PagarCuotaCommand>
{
    public async Task Handle(PagarCuotaCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var credito = await repository.FindByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException(Mensajes.CreditoNotFound);

        ApplyPago(credito, dto.MontoPago);

        logger.LogInformation(LogTemplates.PaymentMade, credito.Id, dto.MontoPago);
        await repository.UpdateAsync(credito);
    }

    private static void ApplyPago(CreditoEntity credito, decimal montoPago)
    {
        credito.Saldo -= montoPago;

        if (credito.Saldo == 0)
            credito.Estado = Domain.Enums.CreditoEstado.Pagado;
    }
}
