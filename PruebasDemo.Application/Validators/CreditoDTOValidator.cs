using FluentValidation;
using PruebasDemo.Application.Resources;
using PruebasDemo.Domain.DTO;

namespace PruebasDemo.Application.Validators
{
    public class CreditoDtoValidator : AbstractValidator<CreditoDto>
    {
        public CreditoDtoValidator()
        {
            RuleFor(x => x.Monto)
                .GreaterThan(0)
                .WithMessage(Mensajes.MontoMustBePositive);

            RuleFor(x => x.TasaInteres)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Mensajes.TasaMustBePositive);

            RuleFor(x => x.Meses)
                .GreaterThan(0)
                .WithMessage(Mensajes.MonthMustBePositive);
        }
    }
}