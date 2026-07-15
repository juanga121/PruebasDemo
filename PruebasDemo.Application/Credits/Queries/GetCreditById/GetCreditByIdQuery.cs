using MediatR;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Application.Credits.Queries.GetCreditById;

public record GetCreditByIdQuery(Guid Id) : IRequest<Credit?>;
