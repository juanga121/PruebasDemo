using FluentValidation.TestHelper;
using Moq;
using PruebasDemo.Application.Credits.Commands.PayInstallment;
using PruebasDemo.Application.Interfaces.Repositories;
using PruebasDemo.Application.Resources;
using PruebasDemo.Domain.DTO;
using PruebasDemo.Domain.Entities;
using PruebasDemo.Domain.Enums;
using PruebaDemoTest.Seeds;

namespace PruebaDemoTest.UnitTests.Credits;

public class PayInstallmentCommandValidatorTest
{
    private readonly Mock<IGenericRepository<PruebasDemo.Domain.Entities.Credit, Guid>> _repositoryMock = new();

    [Fact]
    public async Task Should_HaveError_When_PaymentAmount_Is_Zero()
    {
        _repositoryMock
            .Setup(repository => repository.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Seeded.ActiveCredit);

        var validator = new PayInstallmentCommandValidator(_repositoryMock.Object);
        var command = new PayInstallmentCommand(new PayInstallmentDto { Id = Seeded.CreditId, PaymentAmount = 0 });

        TestValidationResult<PayInstallmentCommand> result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.PaymentAmount)
              .WithErrorMessage(Messages.PaymentMustBePositive);
    }

    [Fact]
    public async Task Should_HaveError_When_Credit_NotFound()
    {
        _repositoryMock
            .Setup(repository => repository.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((PruebasDemo.Domain.Entities.Credit?)null);

        var validator = new PayInstallmentCommandValidator(_repositoryMock.Object);
        var command = new PayInstallmentCommand(new PayInstallmentDto { Id = Guid.NewGuid(), PaymentAmount = 50 });

        TestValidationResult<PayInstallmentCommand> result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor("Dto.Id")
              .WithErrorMessage(Messages.CreditNotFound);
    }

    [Fact]
    public async Task Should_HaveError_When_Credit_NotActive()
    {
        _repositoryMock
            .Setup(repository => repository.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new PruebasDemo.Domain.Entities.Credit {
                Id = Seeded.CreditId,
                Amount = 100,
                Balance = 100,
                Status = CreditStatus.Paid
            });

        var validator = new PayInstallmentCommandValidator(_repositoryMock.Object);
        var command = new PayInstallmentCommand(new PayInstallmentDto { Id = Seeded.CreditId, PaymentAmount = 50 });

        TestValidationResult<PayInstallmentCommand> result = await validator.TestValidateAsync(command);

        result.ShouldHaveAnyValidationError()
              .WithErrorMessage(Messages.CreditNotActive);
    }

    [Fact]
    public async Task Should_HaveError_When_Payment_Exceeds_Balance()
    {
        _repositoryMock
            .Setup(repository => repository.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Seeded.ActiveCredit);

        var validator = new PayInstallmentCommandValidator(_repositoryMock.Object);
        var command = new PayInstallmentCommand(new PayInstallmentDto { Id = Seeded.CreditId, PaymentAmount = 999 });

        TestValidationResult<PayInstallmentCommand> result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor("Dto.PaymentAmount")
              .WithErrorMessage(Messages.PaymentExceedsBalance);
    }

    [Fact]
    public async Task Should_NotHaveErrors_When_Payment_Is_Valid()
    {
        _repositoryMock
            .Setup(repository => repository.FindByIdAsync(Seeded.CreditId))
            .ReturnsAsync(Seeded.ActiveCredit);

        var validator = new PayInstallmentCommandValidator(_repositoryMock.Object);
        var command = new PayInstallmentCommand(Seeded.PartialPayment);

        TestValidationResult<PayInstallmentCommand> result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
