using FluentValidation;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;

namespace PruebasDemo.Application.Validators
{
    public class PagoCreditoDtoValidator : AbstractValidator<PagoCreditoDto>
    {
        private readonly IGenericRepository<CreditoEntity, Guid> _repository;

        public PagoCreditoDtoValidator(IGenericRepository<CreditoEntity, Guid> repository)
        {
            _repository = repository;

            RuleFor(x => x.CreditoId)
                .NotEmpty()
                .WithMessage("El ID del crédito es requerido")
                .MustAsync(CreditoExiste)
                .WithMessage(Mensajes.CreditoNotFound);

            RuleFor(x => x.MontoPago)
                .GreaterThan(0)
                .WithMessage(Mensajes.PaymentMustBePositive);

            RuleFor(x => x)
                .MustAsync(CreditoEstaActivo)
                .WithMessage(Mensajes.CreditoNotActive)
                .When(x => x.CreditoId != Guid.Empty);

            RuleFor(x => x)
                .MustAsync(MontoPagoNoExcedeSaldo)
                .WithMessage(Mensajes.PaymentExceedsBalance)
                .When(x => x.CreditoId != Guid.Empty && x.MontoPago > 0);
        }

        private async Task<bool> CreditoExiste(Guid creditoId, CancellationToken cancellationToken)
        {
            var credito = await _repository.FindByIdAsync(creditoId);
            return credito != null;
        }

        private async Task<bool> CreditoEstaActivo(PagoCreditoDto dto, CancellationToken cancellationToken)
        {
            var credito = await _repository.FindByIdAsync(dto.CreditoId);
            return credito?.Estado == CreditoEstado.Activo;
        }

        private async Task<bool> MontoPagoNoExcedeSaldo(PagoCreditoDto dto, CancellationToken cancellationToken)
        {
            var credito = await _repository.FindByIdAsync(dto.CreditoId);
            return credito != null && dto.MontoPago <= credito.Saldo;
        }
    }
}
