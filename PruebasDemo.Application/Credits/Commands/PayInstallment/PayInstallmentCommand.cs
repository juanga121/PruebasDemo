using MediatR;
using PruebasDemo.Domain.DTO;

namespace PruebasDemo.Application.Credits.Commands.PayInstallment;

public record PayInstallmentCommand(PayInstallmentDto Dto) : IRequest;
