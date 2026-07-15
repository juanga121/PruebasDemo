using MediatR;
using PruebasDemo.Domain.Entities;

namespace PruebasDemo.Application.Credits.Queries.GetCredits;

public record GetCreditsQuery : IRequest<List<Credit>>;
