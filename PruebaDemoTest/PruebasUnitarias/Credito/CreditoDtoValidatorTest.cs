using FluentValidation.TestHelper;
using PruebasDemo.Application.Validators;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Application.Resources;

namespace PruebaDemoTest.PruebasUnitarias.Credito
{
    public class CreditoDtoValidatorTest
    {
        private readonly CreditoDtoValidator _validator = new();

        [Fact]
        public void Debe_Tener_Error_Cuando_Monto_Es_Cero()
        {
            var model = new CreditoDto { Monto = 0 };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Monto)
                  .WithErrorMessage(Mensajes.MontoMustBePositive);
        }

        [Fact]
        public void Debe_Tener_Error_Cuando_TasaInteres_Es_Negativa()
        {
            var model = new CreditoDto { TasaInteres = -1 };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.TasaInteres)
                  .WithErrorMessage(Mensajes.TasaMustBePositive);
        }

        [Fact]
        public void Debe_Tener_Error_Cuando_Meses_Es_Cero()
        {
            var model = new CreditoDto { Meses = 0 };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Meses)
                  .WithErrorMessage(Mensajes.MonthMustBePositive);
        }

        [Fact]
        public void No_Debe_Tener_Errores_Cuando_Modelo_Es_Valido()
        {
            var model = new CreditoDto
            {
                Monto = 1000,
                TasaInteres = 5,
                Meses = 12
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
