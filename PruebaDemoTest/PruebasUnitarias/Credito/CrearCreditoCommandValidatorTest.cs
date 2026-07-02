using FluentValidation.TestHelper;
using PruebasDemo.Application.Creditos.Commands.CrearCredito;
using PruebasDemo.Application.Resources;
using PruebaDemoTest.Seeds;

namespace PruebaDemoTest.PruebasUnitarias.Credito;

public class CrearCreditoCommandValidatorTest
{
    private readonly CrearCreditoCommandValidator _validator = new();

    [Fact]
    public void Debe_Tener_Error_Cuando_Monto_Es_Cero()
    {
        var command = new CrearCreditoCommand(Seeded.ConMontoCero);
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Credito.Monto)
              .WithErrorMessage(Mensajes.MontoMustBePositive);
    }

    [Fact]
    public void Debe_Tener_Error_Cuando_TasaInteres_Es_Negativa()
    {
        var command = new CrearCreditoCommand(Seeded.ConTasaNegativa);
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Credito.TasaInteres)
              .WithErrorMessage(Mensajes.TasaMustBePositive);
    }

    [Fact]
    public void Debe_Tener_Error_Cuando_Meses_Es_Cero()
    {
        var command = new CrearCreditoCommand(Seeded.ConMesesCero);
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Credito.Meses)
              .WithErrorMessage(Mensajes.MonthMustBePositive);
    }

    [Fact]
    public void No_Debe_Tener_Errores_Cuando_Modelo_Es_Valido()
    {
        var command = new CrearCreditoCommand(Seeded.CrearCredito);
        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
