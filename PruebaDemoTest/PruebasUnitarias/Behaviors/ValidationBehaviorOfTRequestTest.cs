using FluentValidation;
using MediatR;
using Moq;
using PruebasDemo.Application.Behaviors;

namespace PruebaDemoTest.PruebasUnitarias.Behaviors;

public record TestCommand(string? Value = null) : IRequest;

public class ValidationBehaviorOfTRequestTest
{
    [Fact]
    public async Task Handle_SinValidators_InvocaNext()
    {
        var validators = Enumerable.Empty<IValidator<TestCommand>>();
        var behavior = new ValidationBehavior<TestCommand>(validators);

        var nextMock = new Mock<RequestHandlerDelegate<Unit>>();
        nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

        var result = await behavior.Handle(new TestCommand(), nextMock.Object, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidadoresPasaron_InvocaNext()
    {
        var validatorMock = new Mock<IValidator<TestCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var behavior = new ValidationBehavior<TestCommand>([validatorMock.Object]);

        var nextMock = new Mock<RequestHandlerDelegate<Unit>>();
        nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

        var result = await behavior.Handle(new TestCommand(), nextMock.Object, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidadoresFallaron_LanzaValidationException()
    {
        var validatorMock = new Mock<IValidator<TestCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult([
                new FluentValidation.Results.ValidationFailure("Value", "Error test")
            ]));

        var behavior = new ValidationBehavior<TestCommand>([validatorMock.Object]);
        var nextMock = new Mock<RequestHandlerDelegate<Unit>>();

        await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(new TestCommand(), nextMock.Object, CancellationToken.None));

        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PasaCancellationTokenANext()
    {
        var validators = Enumerable.Empty<IValidator<TestCommand>>();
        var behavior = new ValidationBehavior<TestCommand>(validators);

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var nextMock = new Mock<RequestHandlerDelegate<Unit>>();
        nextMock.Setup(n => n(token)).ReturnsAsync(Unit.Value);

        await behavior.Handle(new TestCommand(), nextMock.Object, token);

        nextMock.Verify(n => n(token), Times.Once);
    }
}
