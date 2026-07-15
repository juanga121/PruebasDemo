using MediatR;

namespace PruebasDemo.Application.Credits.Commands.DeleteCredit;

public record DeleteCreditCommand(Guid Id) : IRequest;
