using MediatR;
using PruebasDemo.Domain.DTO;

namespace PruebasDemo.Application.Credits.Commands.UpdateCredit;

public record UpdateCreditCommand(Guid Id, CreditDto Credit) : IRequest;
