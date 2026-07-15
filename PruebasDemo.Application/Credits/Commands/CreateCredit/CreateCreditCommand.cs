using MediatR;
using PruebasDemo.Domain.DTO;

namespace PruebasDemo.Application.Credits.Commands.CreateCredit;

public record CreateCreditCommand(CreditDto Credit) : IRequest;
