using FluentValidation.TestHelper;
using Moq;
using PruebasDemo.Application.Creditos.Commands.PagarCuota;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;
using PruebaDemoTest.Seeds;

namespace PruebaDemoTest.PruebasUnitarias.Credito;

public class PagarCuotaCommandValidatorTest
{
    private readonly Mock<IGenericRepository<CreditoEntity, Guid>> _repositoryMock = new();

    [Fact]
    public async Task Debe_Tener_Error_Cuando_MontoPago_Es_Cero()
    {
        _repositoryMock
            .Setup(r => r.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Seeded.CreditoActivo);

        var validator = new PagarCuotaCommandValidator(_repositoryMock.Object);
        var command = new PagarCuotaCommand(new PagarCuotaDto { Id = Seeded.CreditoId, MontoPago = 0 });

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.MontoPago)
              .WithErrorMessage(Mensajes.PaymentMustBePositive);
    }

    [Fact]
    public async Task Debe_Tener_Error_Cuando_Credito_No_Encontrado()
    {
        _repositoryMock
            .Setup(r => r.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((CreditoEntity?)null);

        var validator = new PagarCuotaCommandValidator(_repositoryMock.Object);
        var command = new PagarCuotaCommand(new PagarCuotaDto { Id = Guid.NewGuid(), MontoPago = 50 });

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor("Dto.Id")
              .WithErrorMessage(Mensajes.CreditoNotFound);
    }

    [Fact]
    public async Task Debe_Tener_Error_Cuando_Credito_No_Activo()
    {
        _repositoryMock
            .Setup(r => r.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new CreditoEntity
            {
                Id = Seeded.CreditoId,
                Monto = 100,
                Saldo = 100,
                Estado = CreditoEstado.Pagado
            });

        var validator = new PagarCuotaCommandValidator(_repositoryMock.Object);
        var command = new PagarCuotaCommand(new PagarCuotaDto { Id = Seeded.CreditoId, MontoPago = 50 });

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveAnyValidationError()
              .WithErrorMessage(Mensajes.CreditoNotActive);
    }

    [Fact]
    public async Task Debe_Tener_Error_Cuando_Pago_Excede_Saldo()
    {
        _repositoryMock
            .Setup(r => r.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Seeded.CreditoActivo);

        var validator = new PagarCuotaCommandValidator(_repositoryMock.Object);
        var command = new PagarCuotaCommand(new PagarCuotaDto { Id = Seeded.CreditoId, MontoPago = 999 });

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor("Dto.MontoPago")
              .WithErrorMessage(Mensajes.PaymentExceedsBalance);
    }

    [Fact]
    public async Task No_Debe_Tener_Errores_Cuando_Pago_Es_Valido()
    {
        _repositoryMock
            .Setup(r => r.FindByIdAsync(Seeded.CreditoId))
            .ReturnsAsync(Seeded.CreditoActivo);

        var validator = new PagarCuotaCommandValidator(_repositoryMock.Object);
        var command = new PagarCuotaCommand(Seeded.PagoParcial);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
