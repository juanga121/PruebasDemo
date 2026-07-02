using FluentValidation;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;

namespace PruebasDemo.Application.Creditos.Commands.PagarCuota;

public class PagarCuotaCommandValidator : AbstractValidator<PagarCuotaCommand>
{
    public PagarCuotaCommandValidator(IGenericRepository<CreditoEntity, Guid> repository)
    {
        RuleFor(x => x.Dto.MontoPago)
            .GreaterThan(0)
            .WithMessage(Mensajes.PaymentMustBePositive);

        RuleFor(x => x).CustomAsync(async (command, context, _) =>
        {
            if (command.Dto.MontoPago <= 0) return;

            var credito = await repository.FindByIdAsync(command.Dto.Id);

            ValidarCreditoExiste(credito, context);
            ValidarCreditoActivo(credito, context);
            ValidarMontoPago(command.Dto, credito, context);
        });
    }

    private static void ValidarCreditoExiste(CreditoEntity? credito, ValidationContext<PagarCuotaCommand> context)
    {
        if (credito is null)
            context.AddFailure(nameof(PagarCuotaCommand.Dto) + "." + nameof(PagarCuotaDto.Id), Mensajes.CreditoNotFound);
    }

    private static void ValidarCreditoActivo(CreditoEntity? credito, ValidationContext<PagarCuotaCommand> context)
    {
        if (credito is not null && credito.Estado != CreditoEstado.Activo)
            context.AddFailure(Mensajes.CreditoNotActive);
    }

    private static void ValidarMontoPago(PagarCuotaDto dto, CreditoEntity? credito, ValidationContext<PagarCuotaCommand> context)
    {
        if (credito is not null && dto.MontoPago > credito.Saldo)
            context.AddFailure(nameof(PagarCuotaCommand.Dto) + "." + nameof(PagarCuotaDto.MontoPago), Mensajes.PaymentExceedsBalance);
    }
}
