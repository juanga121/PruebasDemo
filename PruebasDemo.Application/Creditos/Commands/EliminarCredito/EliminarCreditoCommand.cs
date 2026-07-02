using MediatR;

namespace PruebasDemo.Application.Creditos.Commands.EliminarCredito;

public record EliminarCreditoCommand(Guid Id) : IRequest;
