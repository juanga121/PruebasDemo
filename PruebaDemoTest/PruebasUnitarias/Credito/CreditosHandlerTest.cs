using Microsoft.Extensions.Logging;
using Moq;
using PruebasDemo.Application.Creditos.Commands.ActualizarCredito;
using PruebasDemo.Application.Creditos.Commands.CrearCredito;
using PruebasDemo.Application.Creditos.Commands.EliminarCredito;
using PruebasDemo.Application.Creditos.Commands.PagarCuota;
using PruebasDemo.Application.Creditos.Queries.ObtenerCreditoPorId;
using PruebasDemo.Application.Creditos.Queries.ObtenerCreditos;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;
using PruebaDemoTest.Seeds;

namespace PruebaDemoTest.PruebasUnitarias.Credito;

public class CreditosHandlerTest
{
    private readonly Mock<IGenericRepository<CreditoEntity, Guid>> _repositoryMock;

    public CreditosHandlerTest()
    {
        _repositoryMock = new Mock<IGenericRepository<CreditoEntity, Guid>>();
    }

    #region CrearCredito

    [Fact]
    public async Task CrearCredito_DebeGuardarCreditoConValoresCorrectos()
    {
        var dto = Seeded.CrearCredito;

        CreditoEntity? savedEntity = null;

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<CreditoEntity>()))
            .Callback<CreditoEntity>(c => savedEntity = c)
            .Returns(Task.CompletedTask);

        var handler = new CrearCreditoCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<CrearCreditoCommandHandler>>());

        await handler.Handle(new CrearCreditoCommand(dto), default);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<CreditoEntity>()), Times.Once);

        Assert.NotNull(savedEntity);
        Assert.NotEqual(Guid.Empty, savedEntity!.Id);
        Assert.Equal(dto.Monto, savedEntity.Monto);
        Assert.Equal(dto.Monto, savedEntity.Saldo);
        Assert.Equal(dto.TasaInteres, savedEntity.TasaInteres);
        Assert.Equal(dto.Meses, savedEntity.Meses);
        Assert.Equal(CreditoEstado.Activo, savedEntity.Estado);
    }

    #endregion

    #region PagarCuota

    [Fact]
    public async Task PagarCuota_PagoParcial_RestaSaldoYPermaneceActivo()
    {
        var credito = Seeded.CreditoActivo;

        _repositoryMock.Setup(r => r.FindByIdAsync(Seeded.CreditoId))
            .ReturnsAsync(credito);

        var handler = new PagarCuotaCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<PagarCuotaCommandHandler>>());

        await handler.Handle(
            new PagarCuotaCommand(Seeded.PagoParcial), default);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<CreditoEntity>(c =>
            c.Id == Seeded.CreditoId &&
            c.Saldo == 70 &&
            c.Estado == CreditoEstado.Activo
        )), Times.Once);
    }

    [Fact]
    public async Task PagarCuota_PagoExacto_SaldoCero_EstadoPagado()
    {
        var credito = Seeded.CreditoSaldo50;

        _repositoryMock.Setup(r => r.FindByIdAsync(Seeded.PagarId))
            .ReturnsAsync(credito);

        var handler = new PagarCuotaCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<PagarCuotaCommandHandler>>());

        await handler.Handle(
            new PagarCuotaCommand(Seeded.PagoExacto), default);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<CreditoEntity>(c =>
            c.Id == Seeded.PagarId &&
            c.Saldo == 0 &&
            c.Estado == CreditoEstado.Pagado
        )), Times.Once);
    }

    [Fact]
    public async Task PagarCuota_CreditoNoEncontrado_LanzaKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock.Setup(r => r.FindByIdAsync(id))
            .ReturnsAsync((CreditoEntity?)null);

        var handler = new PagarCuotaCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<PagarCuotaCommandHandler>>());

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(
                new PagarCuotaCommand(Seeded.Pago(10, id)), default));

        Assert.Equal(Mensajes.CreditoNotFound, ex.Message);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
    }

    #endregion

    #region ActualizarCredito

    [Fact]
    public async Task ActualizarCredito_DebeActualizarDatos()
    {
        var credito = Seeded.CreditoActivo;
        var dto = Seeded.ActualizarCredito;

        _repositoryMock
            .Setup(r => r.FindByIdAsync(Seeded.CreditoId))
            .ReturnsAsync(credito);

        var handler = new ActualizarCreditoCommandHandler(_repositoryMock.Object);

        await handler.Handle(new ActualizarCreditoCommand(Seeded.CreditoId, dto), default);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<CreditoEntity>(c =>
            c.Monto == dto.Monto &&
            c.TasaInteres == dto.TasaInteres &&
            c.Meses == dto.Meses
        )), Times.Once);
    }

    [Fact]
    public async Task ActualizarCredito_NoEncontrado_LanzaKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.FindByIdAsync(id))
            .ReturnsAsync((CreditoEntity?)null);

        var handler = new ActualizarCreditoCommandHandler(_repositoryMock.Object);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new ActualizarCreditoCommand(id, new CreditoDto()), default));

        Assert.Equal(Mensajes.CreditoNotFound, ex.Message);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CreditoEntity>()), Times.Never);
    }

    #endregion

    #region EliminarCredito

    [Fact]
    public async Task EliminarCredito_DebeEliminarCredito()
    {
        var credito = Seeded.CreditoActivo;

        _repositoryMock
            .Setup(r => r.FindByIdAsync(Seeded.CreditoId))
            .ReturnsAsync(credito);

        var handler = new EliminarCreditoCommandHandler(_repositoryMock.Object);

        await handler.Handle(new EliminarCreditoCommand(Seeded.CreditoId), default);

        _repositoryMock.Verify(r => r.DeleteAsync(Seeded.CreditoId), Times.Once);
    }

    [Fact]
    public async Task EliminarCredito_NoEncontrado_LanzaKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.FindByIdAsync(id))
            .ReturnsAsync((CreditoEntity?)null);

        var handler = new EliminarCreditoCommandHandler(_repositoryMock.Object);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new EliminarCreditoCommand(id), default));

        Assert.Equal(Mensajes.CreditoNotFound, ex.Message);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    #endregion

    #region ObtenerCreditos

    [Fact]
    public async Task ObtenerCreditos_DebeRetornarLista()
    {
        var lista = Seeded.ListaCreditos;

        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(lista);

        var handler = new ObtenerCreditosQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new ObtenerCreditosQuery(), default);

        Assert.Equal(2, result.Count);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    #endregion

    #region ObtenerCreditoPorId

    [Fact]
    public async Task ObtenerCreditoPorId_DebeRetornarCredito()
    {
        var id = Seeded.CreditoId;
        var credito = Seeded.CreditoActivo;

        _repositoryMock
            .Setup(r => r.FindByIdAsync(id))
            .ReturnsAsync(credito);

        var handler = new ObtenerCreditoPorIdQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new ObtenerCreditoPorIdQuery(id), default);

        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
    }

    [Fact]
    public async Task ObtenerCreditoPorId_NoEncontrado_DebeRetornarNull()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.FindByIdAsync(id))
            .ReturnsAsync((CreditoEntity?)null);

        var handler = new ObtenerCreditoPorIdQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new ObtenerCreditoPorIdQuery(id), default);

        Assert.Null(result);
    }

    #endregion
}
