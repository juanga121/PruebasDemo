using MediatR;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Application.Credits.Queries.GetCredits;

public class GetCreditsQueryHandler(
    IGenericRepository<Credit, Guid> repository)
    : IRequestHandler<GetCreditsQuery, List<Credit>>
{
    public async Task<List<Credit>> Handle(GetCreditsQuery request, CancellationToken cancellationToken)
        => await repository.GetAllAsync();
}
