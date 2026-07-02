using MediatR;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Application.Creditos.Queries.ObtenerCreditoPorId;

public class ObtenerCreditoPorIdQueryHandler(
    IGenericRepository<CreditoEntity, Guid> repository)
    : IRequestHandler<ObtenerCreditoPorIdQuery, CreditoEntity?>
{
    public async Task<CreditoEntity?> Handle(ObtenerCreditoPorIdQuery request, CancellationToken cancellationToken)
        => await repository.FindByIdAsync(request.Id);
}
