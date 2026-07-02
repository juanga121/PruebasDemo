using MediatR;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Application.Creditos.Queries.ObtenerCreditos;

public record ObtenerCreditosQuery : IRequest<List<CreditoEntity>>;
