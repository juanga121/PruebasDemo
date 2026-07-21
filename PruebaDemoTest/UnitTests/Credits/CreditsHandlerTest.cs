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
        CreditDto createRequest = CreditSeeds.CreateCreditDto;

        PruebasDemo.Domain.Entities.Credit? savedEntity = null;

        _repositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<PruebasDemo.Domain.Entities.Credit>()))
            .Callback<PruebasDemo.Domain.Entities.Credit>(credit => savedEntity = credit)
            .Returns(Task.CompletedTask);

        CreateCreditCommandHandler handler = new CreateCreditCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<CreateCreditCommandHandler>>());

        await handler.Handle(new CreateCreditCommand(createRequest), default);

        _repositoryMock.Verify(repository => repository.CreateAsync(It.IsAny<PruebasDemo.Domain.Entities.Credit>()), Times.Once);

        Assert.NotNull(savedEntity);
        Assert.NotEqual(Guid.Empty, savedEntity!.Id);
        Assert.Equal(createRequest.Amount, savedEntity.Amount);
        Assert.Equal(createRequest.Amount, savedEntity.Balance);
        Assert.Equal(createRequest.InterestRate, savedEntity.InterestRate);
        Assert.Equal(createRequest.Months, savedEntity.Months);
        Assert.Equal(CreditStatus.Active, savedEntity.Status);
    }

    #endregion

    #region PayInstallment

    [Fact]
    public async Task PayInstallment_PartialPayment_ReducesBalanceAndRemainsActive()
    {
        Credit credit = CreditSeeds.ActiveCredit;

        _repositoryMock.Setup(repository => repository.FindByIdAsync(CreditSeeds.CreditId))
            .ReturnsAsync(credit);

        var handler = new PayInstallmentCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<PayInstallmentCommandHandler>>());

        await handler.Handle(
            new PayInstallmentCommand(PaymentSeeds.PartialPayment), default);

        _repositoryMock.Verify(repository => repository.UpdateAsync(It.Is<PruebasDemo.Domain.Entities.Credit>(credit =>
            credit.Id == CreditSeeds.CreditId &&
            credit.Balance == 70 &&
            credit.Status == CreditStatus.Active
        )), Times.Once);
    }

    [Fact]
    public async Task PayInstallment_ExactPayment_BalanceZero_StatusPaid()
    {
        Credit credit = CreditSeeds.CreditWithBalance50;

        _repositoryMock.Setup(repository => repository.FindByIdAsync(PaymentSeeds.PayId))
            .ReturnsAsync(credit);

        var handler = new PayInstallmentCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<PayInstallmentCommandHandler>>());

        await handler.Handle(
            new PayInstallmentCommand(PaymentSeeds.ExactPayment), default);

        _repositoryMock.Verify(repository => repository.UpdateAsync(It.Is<PruebasDemo.Domain.Entities.Credit>(credit =>
            credit.Id == PaymentSeeds.PayId &&
            credit.Balance == 0 &&
            credit.Status == CreditStatus.Paid
        )), Times.Once);
    }

    [Fact]
    public async Task PayInstallment_CreditNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock.Setup(repository => repository.FindByIdAsync(id))
            .ReturnsAsync((PruebasDemo.Domain.Entities.Credit?)null);

        var handler = new PayInstallmentCommandHandler(
            _repositoryMock.Object,
            Mock.Of<ILogger<PayInstallmentCommandHandler>>());

        KeyNotFoundException exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(
                new PayInstallmentCommand(PaymentSeeds.Payment(10, id)), default));

        Assert.Equal(Messages.CreditNotFound, exception.Message);
        _repositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<PruebasDemo.Domain.Entities.Credit>()), Times.Never);
    }

    #endregion

    #region UpdateCredit

    [Fact]
    public async Task UpdateCredit_ShouldUpdateData()
    {
        Credit credit = CreditSeeds.ActiveCredit;
        CreditDto updateRequest = CreditSeeds.UpdateCreditDto;

        _repositoryMock
            .Setup(repository => repository.FindByIdAsync(CreditSeeds.CreditId))
            .ReturnsAsync(credit);

        UpdateCreditCommandHandler handler = new UpdateCreditCommandHandler(_repositoryMock.Object);

        await handler.Handle(new UpdateCreditCommand(CreditSeeds.CreditId, updateRequest), default);

        _repositoryMock.Verify(repository => repository.UpdateAsync(It.Is<PruebasDemo.Domain.Entities.Credit>(credit =>
            credit.Amount == updateRequest.Amount &&
            credit.InterestRate == updateRequest.InterestRate &&
            credit.Months == updateRequest.Months
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateCredit_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(repository => repository.FindByIdAsync(id))
            .ReturnsAsync((PruebasDemo.Domain.Entities.Credit?)null);

        var handler = new UpdateCreditCommandHandler(_repositoryMock.Object);

        KeyNotFoundException exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new UpdateCreditCommand(id, new CreditDto()), default));

        Assert.Equal(Messages.CreditNotFound, exception.Message);
        _repositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<PruebasDemo.Domain.Entities.Credit>()), Times.Never);
    }

    #endregion

    #region DeleteCredit

    [Fact]
    public async Task DeleteCredit_ShouldDeleteCredit()
    {
        Credit credit = CreditSeeds.ActiveCredit;

        _repositoryMock
            .Setup(repository => repository.FindByIdAsync(CreditSeeds.CreditId))
            .ReturnsAsync(credit);

        var handler = new DeleteCreditCommandHandler(_repositoryMock.Object);

        await handler.Handle(new DeleteCreditCommand(CreditSeeds.CreditId), default);

        _repositoryMock.Verify(repository => repository.DeleteAsync(CreditSeeds.CreditId), Times.Once);
    }

    [Fact]
    public async Task DeleteCredit_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(repository => repository.FindByIdAsync(id))
            .ReturnsAsync((PruebasDemo.Domain.Entities.Credit?)null);

        var handler = new DeleteCreditCommandHandler(_repositoryMock.Object);

        KeyNotFoundException exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteCreditCommand(id), default));

        Assert.Equal(Messages.CreditNotFound, exception.Message);
        _repositoryMock.Verify(repository => repository.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    #endregion

    #region GetCredits

    [Fact]
    public async Task GetCredits_ShouldReturnList()
    {
        List<Credit> list = CreditSeeds.CreditsList;

        _repositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(list);

        var handler = new GetCreditsQueryHandler(_repositoryMock.Object);

        List<Credit> result = await handler.Handle(new GetCreditsQuery(), default);

        Assert.Equal(2, result.Count);
        _repositoryMock.Verify(repository => repository.GetAllAsync(), Times.Once);
    }

    #endregion

    #region GetCreditById

    [Fact]
    public async Task GetCreditById_ShouldReturnCredit()
    {
        var id = CreditSeeds.CreditId;
        Credit credit = CreditSeeds.ActiveCredit;

        _repositoryMock
            .Setup(repository => repository.FindByIdAsync(id))
            .ReturnsAsync(credit);

        var handler = new GetCreditByIdQueryHandler(_repositoryMock.Object);

        Credit? result = await handler.Handle(new GetCreditByIdQuery(id), default);

        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
    }

    [Fact]
    public async Task GetCreditById_NotFound_ShouldReturnNull()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(repository => repository.FindByIdAsync(id))
            .ReturnsAsync((PruebasDemo.Domain.Entities.Credit?)null);

        var handler = new GetCreditByIdQueryHandler(_repositoryMock.Object);

        Credit? result = await handler.Handle(new GetCreditByIdQuery(id), default);

        Assert.Null(result);
    }

    #endregion
}
