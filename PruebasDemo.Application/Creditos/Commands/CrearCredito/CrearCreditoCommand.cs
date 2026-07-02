using MediatR;
using PruebasDemo.Domain.DTO;

namespace PruebasDemo.Application.Creditos.Commands.CrearCredito;

public record CrearCreditoCommand(CreditoDto Credito) : IRequest;
