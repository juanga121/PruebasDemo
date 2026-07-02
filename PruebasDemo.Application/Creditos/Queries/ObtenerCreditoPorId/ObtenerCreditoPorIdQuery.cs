using MediatR;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Application.Creditos.Queries.ObtenerCreditoPorId;

public record ObtenerCreditoPorIdQuery(Guid Id) : IRequest<CreditoEntity?>;
