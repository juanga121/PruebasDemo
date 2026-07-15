using MediatR;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Application.Credits.Queries.GetCreditById;

public class GetCreditByIdQueryHandler(
    IGenericRepository<Credit, Guid> repository)
    : IRequestHandler<GetCreditByIdQuery, Credit?>
{
    public async Task<Credit?> Handle(GetCreditByIdQuery request, CancellationToken cancellationToken)
        => await repository.FindByIdAsync(request.Id);
}
