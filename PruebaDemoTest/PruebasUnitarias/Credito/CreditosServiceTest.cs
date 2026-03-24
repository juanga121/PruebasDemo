using Moq;
using PruebasDemo.Application.Repositories;
using PruebasDemo.Application.Services;
using PruebasDemo.Domain.Entities;

namespace PruebaDemoTest.PruebasUnitarias.Credito
{
    public class CreditosServiceTest
    {
        private readonly Mock<IGenericRepository<CreditoEntity, Guid>> _repositoryMock;

        public CreditosServiceTest()
        {
            _repositoryMock = new Mock<IGenericRepository<CreditoEntity, Guid>>();
        }

        [Fact]
        public async Task PagarCuota_PagoParcial_RestaSaldoYPermaneceActivo()
        {
            // Arrange
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = 100,
                Saldo = 100,
                Estado = 1
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            var service = new CreditosService(_repositoryMock.Object);

            // Act
            await service.PagarCuota(id, 30);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<CreditoEntity>(c =>
                c.Id == id &&
                c.Saldo == 70 &&
                c.Estado == 1
            )), Times.Once);
        }

        [Fact]
        public async Task PagarCuota_PagoExacto_SaldoCero_Estado2()
        {
            // Arrange
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = 50,
                Saldo = 50,
                Estado = 1
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            var service = new CreditosService(_repositoryMock.Object);

            // Act
            await service.PagarCuota(id, 50);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<CreditoEntity>(c =>
                c.Id == id &&
                c.Saldo == 0 &&
                c.Estado == 2
            )), Times.Once);
        }

        [Fact]
        public async Task PagarCuota_CreditoNoEncontrado_LanzaException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync((CreditoEntity?)null);

            var service = new CreditosService(_repositoryMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.PagarCuota(id, 10));
            Assert.Equal("Crédito no encontrado", ex.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }

        [Fact]
        public async Task PagarCuota_CreditoNoActivo_LanzaException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = 100,
                Saldo = 100,
                Estado = 3
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            var service = new CreditosService(_repositoryMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.PagarCuota(id, 10));
            Assert.Equal("El crédito no está activo", ex.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }

        [Fact]
        public async Task PagarCuota_MontoInvalido_CeroONegativo_LanzaException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = 100,
                Saldo = 100,
                Estado = 1
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            var service = new CreditosService(_repositoryMock.Object);

            // Act & Assert
            var exZero = await Assert.ThrowsAsync<Exception>(() => service.PagarCuota(id, 0));
            Assert.Equal("El monto de pago debe ser mayor a cero", exZero.Message);

            // Act & Assert
            var exNeg = await Assert.ThrowsAsync<Exception>(() => service.PagarCuota(id, -5));
            Assert.Equal("El monto de pago debe ser mayor a cero", exNeg.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }

        [Fact]
        public async Task PagarCuota_MontoMayorQueSaldo_LanzaException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var credito = new CreditoEntity
            {
                Id = id,
                Monto = 100,
                Saldo = 40,
                Estado = 1
            };

            _repositoryMock.Setup(r => r.FindByIdAsync(id))
                .ReturnsAsync(credito);

            var service = new CreditosService(_repositoryMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.PagarCuota(id, 50));
            Assert.Equal("El monto de pago excede el saldo del crédito", ex.Message);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
        }
    }
}
