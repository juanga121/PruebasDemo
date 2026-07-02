using MediatR;
using PruebasDemo.Domain.DTO;

namespace PruebasDemo.Application.Creditos.Commands.PagarCuota;

public record PagarCuotaCommand(PagarCuotaDto Dto) : IRequest;
