using Microsoft.Extensions.Logging;
using Moq;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Services;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;

namespace PruebaDemoTest.PruebasUnitarias.Credito
{
    public class CreditosServiceTest
    {
        private readonly Mock<IGenericRepository<CreditoEntity, Guid>> _repositoryMock;
        private readonly Mock<ILogger<CreditosService>> _loggerMock;

        public CreditosServiceTest()
        {
            _repositoryMock = new Mock<IGenericRepository<CreditoEntity, Guid>>();
            _loggerMock = new Mock<ILogger<CreditosService>>();
        }

        [Fact]
        public async Task PagarCuota_PagoParcial_RestaSaldoYPermaneceActivo()
        {
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = 100,
                Saldo = 100,
                Estado = CreditoEstado.Activo
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object);

            await service.PagarCuota(id, 30);

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

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object);

            await service.PagarCuota(id, 50);

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

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.PagarCuota(id, 10));
            Assert.Equal("Crédito no encontrado", ex.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }

        [Fact]
        public async Task PagarCuota_CreditoNoActivo_LanzaException()
        {
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = 100,
                Saldo = 100,
                Estado = CreditoEstado.Inactivo
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PagarCuota(id, 10));
            Assert.Equal("El crédito no está activo", ex.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }

        [Fact]
        public async Task PagarCuota_MontoInvalido_CeroONegativo_LanzaException()
        {
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = 100,
                Saldo = 100,
                Estado = CreditoEstado.Activo
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object);

            var exZero = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PagarCuota(id, 0));
            Assert.Equal("El monto de pago debe ser mayor a cero", exZero.Message);

            var exNeg = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PagarCuota(id, -5));
            Assert.Equal("El monto de pago debe ser mayor a cero", exNeg.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }

        [Fact]
        public async Task PagarCuota_MontoMayorQueSaldo_LanzaException()
        {
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = 100,
                Saldo = 40,
                Estado = CreditoEstado.Activo
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PagarCuota(id, 50));
            Assert.Equal("El monto de pago excede el saldo del crédito", ex.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }

        [Fact]
        public async Task CrearCredito_DebeGuardarCredito()
        {
            var dto = new CreditoDto
            {
                Monto = 100,
                TasaInteres = 10,
                Meses = 12
            };

            CreditoEntity? savedEntity = null;

            _repositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<CreditoEntity>()))
                .Callback<CreditoEntity>(c => savedEntity = c)
                .Returns(Task.CompletedTask);

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object);

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
                new() { Id = Guid.NewGuid(), Monto = 100 },
                new() { Id = Guid.NewGuid(), Monto = 200 }
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(lista);

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object);

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

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object);

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
                Monto = 100,
                TasaInteres = 5,
                Meses = 12
            };

            var dto = new CreditoDto
            {
                Monto = 200,
                TasaInteres = 10,
                Meses = 24
            };

            _repositoryMock
                .Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object);

            await service.ActualizarCredito(id, dto);

            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<CreditoEntity>(c =>
                c.Monto == 200 &&
                c.TasaInteres == 10 &&
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

            var service = new CreditosService(_repositoryMock.Object, _loggerMock.Object);

            await service.EliminarCredito(id);

            _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
