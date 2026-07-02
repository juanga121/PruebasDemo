using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Application.Services;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;
using PruebaDemoTest.Constants;

namespace PruebaDemoTest.PruebasUnitarias.Credito
{
    public class CreditosServiceTest
    {
        private readonly Mock<IGenericRepository<CreditoEntity, Guid>> _repositoryMock;
        private readonly Mock<ILogger<CreditosService>> _loggerMock;
        private readonly Mock<IValidator<PagarCuotaDto>> _pagarCuotaValidatorMock;

        public CreditosServiceTest()
        {
            _repositoryMock = new Mock<IGenericRepository<CreditoEntity, Guid>>();
            _loggerMock = new Mock<ILogger<CreditosService>>();
            _pagarCuotaValidatorMock = new Mock<IValidator<PagarCuotaDto>>();
        }

        [Fact]
        public async Task PagarCuota_PagoParcial_RestaSaldoYPermaneceActivo()
        {
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = TestConstants.MontoDefault,
                Saldo = TestConstants.SaldoDefault,
                Estado = CreditoEstado.Activo
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            _pagarCuotaValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PagarCuotaDto>(), default))
                .ReturnsAsync(new ValidationResult());

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object, _pagarCuotaValidatorMock.Object);

            await service.PagarCuota(new PagarCuotaDto { Id = id, MontoPago = TestConstants.MontoPagoParcial });

            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<CreditoEntity>(c =>
                c.Id == id &&
                c.Saldo == 70 &&
                c.Estado == CreditoEstado.Activo
            )), Times.Once);
        }

        [Fact]
        public async Task PagarCuota_PagoExacto_SaldoCero_Estado2()
        {
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = 50,
                Saldo = 50,
                Estado = CreditoEstado.Activo
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            _pagarCuotaValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PagarCuotaDto>(), default))
                .ReturnsAsync(new ValidationResult());

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object, _pagarCuotaValidatorMock.Object);

            await service.PagarCuota(new PagarCuotaDto { Id = id, MontoPago = TestConstants.MontoPagoExacto });

            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<CreditoEntity>(c =>
                c.Id == id &&
                c.Saldo == 0 &&
                c.Estado == CreditoEstado.Pagado
            )), Times.Once);
        }

        [Fact]
        public async Task PagarCuota_CreditoNoEncontrado_LanzaException()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync((CreditoEntity?)null);

            _pagarCuotaValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PagarCuotaDto>(), default))
                .ReturnsAsync(new ValidationResult());

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object, _pagarCuotaValidatorMock.Object);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.PagarCuota(new PagarCuotaDto { Id = id, MontoPago = 10 }));
            Assert.Equal(Mensajes.CreditoNotFound, ex.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }

        [Fact]
        public async Task PagarCuota_CreditoNoActivo_LanzaException()
        {
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = TestConstants.MontoDefault,
                Saldo = TestConstants.SaldoDefault,
                Estado = CreditoEstado.Inactivo
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            _pagarCuotaValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PagarCuotaDto>(), default))
                .ReturnsAsync(new ValidationResult());

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object, _pagarCuotaValidatorMock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PagarCuota(new PagarCuotaDto { Id = id, MontoPago = 10 }));
            Assert.Equal(Mensajes.CreditoNotActive, ex.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }

        [Fact]
        public async Task PagarCuota_MontoInvalido_CeroONegativo_LanzaException()
        {
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = TestConstants.MontoDefault,
                Saldo = TestConstants.SaldoDefault,
                Estado = CreditoEstado.Activo
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            _pagarCuotaValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<PagarCuotaDto>(), default))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                {
                    new("MontoPago", Mensajes.PaymentMustBePositive)
                }));

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object, _pagarCuotaValidatorMock.Object);

            var exZero = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PagarCuota(new PagarCuotaDto { Id = id, MontoPago = 0 }));
            Assert.Equal(Mensajes.PaymentMustBePositive, exZero.Message);

            var exNeg = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PagarCuota(new PagarCuotaDto { Id = id, MontoPago = -5 }));
            Assert.Equal(Mensajes.PaymentMustBePositive, exNeg.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }

        [Fact]
        public async Task PagarCuota_MontoMayorQueSaldo_LanzaException()
        {
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = TestConstants.MontoDefault,
                Saldo = 40,
                Estado = CreditoEstado.Activo
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            _pagarCuotaValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PagarCuotaDto>(), default))
                .ReturnsAsync(new ValidationResult());

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object, _pagarCuotaValidatorMock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PagarCuota(new PagarCuotaDto { Id = id, MontoPago = TestConstants.MontoPagoExacto }));
            Assert.Equal(Mensajes.PaymentExceedsBalance, ex.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }

        [Fact]
        public async Task CrearCredito_DebeGuardarCredito()
        {
            var dto = new CreditoDto
            {
                Monto = TestConstants.MontoDefault,
                TasaInteres = TestConstants.TasaInteresDefault,
                Meses = TestConstants.MesesDefault
            };

            CreditoEntity? savedEntity = null;

            _repositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<CreditoEntity>()))
                .Callback<CreditoEntity>(c => savedEntity = c)
                .Returns(Task.CompletedTask);

            _pagarCuotaValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PagarCuotaDto>(), default))
                .ReturnsAsync(new ValidationResult());

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object, _pagarCuotaValidatorMock.Object);

            await service.CrearCredito(dto);

            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<CreditoEntity>()), Times.Once);

            Assert.NotNull(savedEntity);
            Assert.Equal(dto.Monto, savedEntity!.Monto);
            Assert.Equal(dto.Monto, savedEntity.Saldo);
            Assert.Equal(CreditoEstado.Activo, savedEntity.Estado);
        }

        [Fact]
        public async Task ObtenerCreditos_DebeRetornarLista()
        {
            var lista = new List<CreditoEntity>
            {
                new() { Id = Guid.NewGuid(), Monto = TestConstants.MontoDefault },
                new() { Id = Guid.NewGuid(), Monto = 200 }
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(lista);

            _pagarCuotaValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PagarCuotaDto>(), default))
                .ReturnsAsync(new ValidationResult());

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object, _pagarCuotaValidatorMock.Object);

            var result = await service.ObtenerCreditos();

            Assert.Equal(2, result.Count);
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ObtenerCreditoPorId_DebeRetornarCredito()
        {
            var id = Guid.NewGuid();
            var credito = new CreditoEntity { Id = id };

            _repositoryMock
                .Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            _pagarCuotaValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PagarCuotaDto>(), default))
                .ReturnsAsync(new ValidationResult());

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object, _pagarCuotaValidatorMock.Object);

            var result = await service.ObtenerCreditoPorId(id);

            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);
        }

        [Fact]
        public async Task ActualizarCredito_DebeActualizarDatos()
        {
            var id = Guid.NewGuid();

            var credito = new CreditoEntity
            {
                Id = id,
                Monto = TestConstants.MontoDefault,
                TasaInteres = 5,
                Meses = TestConstants.MesesDefault
            };

            var dto = new CreditoDto
            {
                Monto = 200,
                TasaInteres = TestConstants.TasaInteresDefault,
                Meses = 24
            };

            _repositoryMock
                .Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            _pagarCuotaValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PagarCuotaDto>(), default))
                .ReturnsAsync(new ValidationResult());

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object, _pagarCuotaValidatorMock.Object);

            await service.ActualizarCredito(id, dto);

            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<CreditoEntity>(c =>
                c.Monto == 200 &&
                c.TasaInteres == TestConstants.TasaInteresDefault &&
                c.Meses == 24
            )), Times.Once);
        }

        [Fact]
        public async Task EliminarCredito_DebeEliminarCredito()
        {
            var id = Guid.NewGuid();

            var credito = new CreditoEntity { Id = id };

            _repositoryMock
                .Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            _pagarCuotaValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PagarCuotaDto>(), default))
                .ReturnsAsync(new ValidationResult());

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object, _pagarCuotaValidatorMock.Object);

            await service.EliminarCredito(id);

            _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
