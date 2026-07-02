using FluentValidation;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;

namespace PruebasDemo.Application.Validators
{
    public class PagarCuotaDtoValidator : AbstractValidator<PagarCuotaDto>
    {
        private readonly IGenericRepository<CreditoEntity, Guid> _repository;

        public PagarCuotaDtoValidator(IGenericRepository<CreditoEntity, Guid> repository)
        {
            _repository = repository;

            RuleFor(x => x.MontoPago)
                .GreaterThan(0)
                .WithMessage(Mensajes.PaymentMustBePositive);
        }
    }
}
