using Microsoft.Extensions.Logging;
using Moq;
using PruebasDemo.Application.Credits.Commands.CreateCredit;
using PruebasDemo.Application.Credits.Commands.DeleteCredit;
using PruebasDemo.Application.Credits.Commands.PayInstallment;
using PruebasDemo.Application.Credits.Commands.UpdateCredit;
using PruebasDemo.Application.Credits.Queries.GetCreditById;
using PruebasDemo.Application.Credits.Queries.GetCredits;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;
using PruebaDemoTest.Seeds;

namespace PruebaDemoTest.UnitTests.Credits;

public class CreditsHandlerTest
{
    private readonly Mock<IGenericRepository<PruebasDemo.Domain.Entities.Credit, Guid>> _repositoryMock;

    public CreditsHandlerTest()
    {
        _repositoryMock = new Mock<IGenericRepository<PruebasDemo.Domain.Entities.Credit, Guid>>();
    }

    #region CreateCredit

    [Fact]
    public async Task CreateCredit_ShouldSaveWithCorrectValues()
    {
        var dto = Seeded.CreateCreditDto;

        PruebasDemo.Domain.Entities.Credit? savedEntity = null;

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<PruebasDemo.Domain.Entities.Credit>()))
            .Callback<PruebasDemo.Domain.Entities.Credit>(c => savedEntity = c)
            .Returns(Task.CompletedTask);

        var handler = new CreateCreditCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<CreateCreditCommandHandler>>());

        await handler.Handle(new CreateCreditCommand(dto), default);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<PruebasDemo.Domain.Entities.Credit>()), Times.Once);

        Assert.NotNull(savedEntity);
        Assert.NotEqual(Guid.Empty, savedEntity!.Id);
        Assert.Equal(dto.Amount, savedEntity.Amount);
        Assert.Equal(dto.Amount, savedEntity.Balance);
        Assert.Equal(dto.InterestRate, savedEntity.InterestRate);
        Assert.Equal(dto.Months, savedEntity.Months);
        Assert.Equal(CreditStatus.Active, savedEntity.Status);
    }

    #endregion

    #region PayInstallment

    [Fact]
    public async Task PayInstallment_PartialPayment_ReducesBalanceAndRemainsActive()
    {
        var credit = Seeded.ActiveCredit;

        _repositoryMock.Setup(r => r.FindByIdAsync(Seeded.CreditId))
            .ReturnsAsync(credit);

        var handler = new PayInstallmentCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<PayInstallmentCommandHandler>>());

        await handler.Handle(
            new PayInstallmentCommand(Seeded.PartialPayment), default);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<PruebasDemo.Domain.Entities.Credit>(c =>
            c.Id == Seeded.CreditId &&
            c.Balance == 70 &&
            c.Status == CreditStatus.Active
        )), Times.Once);
    }

    [Fact]
    public async Task PayInstallment_ExactPayment_BalanceZero_StatusPaid()
    {
        var credit = Seeded.CreditWithBalance50;

        _repositoryMock.Setup(r => r.FindByIdAsync(Seeded.PayId))
            .ReturnsAsync(credit);

        var handler = new PayInstallmentCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<PayInstallmentCommandHandler>>());

        await handler.Handle(
            new PayInstallmentCommand(Seeded.ExactPayment), default);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<PruebasDemo.Domain.Entities.Credit>(c =>
            c.Id == Seeded.PayId &&
            c.Balance == 0 &&
            c.Status == CreditStatus.Paid
        )), Times.Once);
    }

    [Fact]
    public async Task PayInstallment_CreditNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock.Setup(r => r.FindByIdAsync(id))
            .ReturnsAsync((PruebasDemo.Domain.Entities.Credit?)null);

        var handler = new PayInstallmentCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<PayInstallmentCommandHandler>>());

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(
                new PayInstallmentCommand(Seeded.Payment(10, id)), default));

        Assert.Equal(Messages.CreditNotFound, ex.Message);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PruebasDemo.Domain.Entities.Credit>()), Times.Never);
    }

    #endregion

    #region UpdateCredit

    [Fact]
    public async Task UpdateCredit_ShouldUpdateData()
    {
        var credit = Seeded.ActiveCredit;
        var dto = Seeded.UpdateCreditDto;

        _repositoryMock
            .Setup(r => r.FindByIdAsync(Seeded.CreditId))
            .ReturnsAsync(credit);

        var handler = new UpdateCreditCommandHandler(_repositoryMock.Object);

        await handler.Handle(new UpdateCreditCommand(Seeded.CreditId, dto), default);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<PruebasDemo.Domain.Entities.Credit>(c =>
            c.Amount == dto.Amount &&
            c.InterestRate == dto.InterestRate &&
            c.Months == dto.Months
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateCredit_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.FindByIdAsync(id))
            .ReturnsAsync((PruebasDemo.Domain.Entities.Credit?)null);

        var handler = new UpdateCreditCommandHandler(_repositoryMock.Object);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new UpdateCreditCommand(id, new CreditDto()), default));

        Assert.Equal(Messages.CreditNotFound, ex.Message);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PruebasDemo.Domain.Entities.Credit>()), Times.Never);
    }

    #endregion

    #region DeleteCredit

    [Fact]
    public async Task DeleteCredit_ShouldDeleteCredit()
    {
        var credit = Seeded.ActiveCredit;

        _repositoryMock
            .Setup(r => r.FindByIdAsync(Seeded.CreditId))
            .ReturnsAsync(credit);

        var handler = new DeleteCreditCommandHandler(_repositoryMock.Object);

        await handler.Handle(new DeleteCreditCommand(Seeded.CreditId), default);

        _repositoryMock.Verify(r => r.DeleteAsync(Seeded.CreditId), Times.Once);
    }

    [Fact]
    public async Task DeleteCredit_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.FindByIdAsync(id))
            .ReturnsAsync((PruebasDemo.Domain.Entities.Credit?)null);

        var handler = new DeleteCreditCommandHandler(_repositoryMock.Object);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteCreditCommand(id), default));

        Assert.Equal(Messages.CreditNotFound, ex.Message);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    #endregion

    #region GetCredits

    [Fact]
    public async Task GetCredits_ShouldReturnList()
    {
        var list = Seeded.CreditsList;

        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(list);

        var handler = new GetCreditsQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetCreditsQuery(), default);

        Assert.Equal(2, result.Count);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    #endregion

    #region GetCreditById

    [Fact]
    public async Task GetCreditById_ShouldReturnCredit()
    {
        var id = Seeded.CreditId;
        var credit = Seeded.ActiveCredit;

        _repositoryMock
            .Setup(r => r.FindByIdAsync(id))
            .ReturnsAsync(credit);

        var handler = new GetCreditByIdQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetCreditByIdQuery(id), default);

        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
    }

    [Fact]
    public async Task GetCreditById_NotFound_ShouldReturnNull()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.FindByIdAsync(id))
            .ReturnsAsync((PruebasDemo.Domain.Entities.Credit?)null);

        var handler = new GetCreditByIdQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetCreditByIdQuery(id), default);

        Assert.Null(result);
    }

    #endregion
}
