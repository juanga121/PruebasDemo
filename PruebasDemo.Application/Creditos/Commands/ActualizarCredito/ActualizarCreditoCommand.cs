using MediatR;
using PruebasDemo.Domain.DTO;

namespace PruebasDemo.Application.Creditos.Commands.ActualizarCredito;

public record ActualizarCreditoCommand(Guid Id, CreditoDto Credito) : IRequest;
