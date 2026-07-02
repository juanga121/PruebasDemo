using MediatR;
using Microsoft.Extensions.Logging;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources.Constants;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;

namespace PruebasDemo.Application.Creditos.Commands.CrearCredito;

public class CrearCreditoCommandHandler(
    IGenericRepository<CreditoEntity, Guid> repository,
    ILogger<CrearCreditoCommandHandler> logger) : IRequestHandler<CrearCreditoCommand>
{
    public async Task Handle(CrearCreditoCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Credito;

        var credito = new CreditoEntity
        {
            Id = Guid.NewGuid(),
            Monto = dto.Monto,
            Saldo = dto.Monto,
            TasaInteres = dto.TasaInteres,
            Meses = dto.Meses,
            Estado = CreditoEstado.Activo
        };

        logger.LogInformation(LogTemplates.CreditCreate, credito.Id);
        await repository.CreateAsync(credito);
    }
}
