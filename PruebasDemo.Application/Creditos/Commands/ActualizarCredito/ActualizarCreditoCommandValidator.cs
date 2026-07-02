using FluentValidation;
using PruebasDemo.Application.Resources;

namespace PruebasDemo.Application.Creditos.Commands.ActualizarCredito;

public class ActualizarCreditoCommandValidator : AbstractValidator<ActualizarCreditoCommand>
{
    public ActualizarCreditoCommandValidator()
    {
        RuleFor(x => x.Credito.Monto)
            .GreaterThan(0)
            .WithMessage(Mensajes.MontoMustBePositive);

        RuleFor(x => x.Credito.TasaInteres)
            .GreaterThanOrEqualTo(0)
            .WithMessage(Mensajes.TasaMustBePositive);

        RuleFor(x => x.Credito.Meses)
            .GreaterThan(0)
            .WithMessage(Mensajes.MonthMustBePositive);
    }
}
