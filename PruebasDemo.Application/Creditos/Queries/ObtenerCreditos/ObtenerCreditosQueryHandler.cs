using MediatR;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Application.Creditos.Queries.ObtenerCreditos;

public class ObtenerCreditosQueryHandler(
    IGenericRepository<CreditoEntity, Guid> repository)
    : IRequestHandler<ObtenerCreditosQuery, List<CreditoEntity>>
{
    public async Task<List<CreditoEntity>> Handle(ObtenerCreditosQuery request, CancellationToken cancellationToken)
        => await repository.GetAllAsync();
}
