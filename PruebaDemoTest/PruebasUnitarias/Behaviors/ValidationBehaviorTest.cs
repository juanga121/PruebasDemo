using FluentValidation;
using MediatR;
using Moq;
using PruebasDemo.Application.Behaviors;

namespace PruebaDemoTest.PruebasUnitarias.Behaviors;

public record TestQuery(string Value) : IRequest<string>;

public class ValidationBehaviorTest
{
    [Fact]
    public async Task Handle_SinValidators_InvocaNext()
    {
        var validators = Enumerable.Empty<IValidator<TestQuery>>();
        var behavior = new ValidationBehavior<TestQuery, string>(validators);

        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync("ok");

        var result = await behavior.Handle(new TestQuery("x"), nextMock.Object, CancellationToken.None);

        Assert.Equal("ok", result);
        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidadoresPasaron_InvocaNext()
    {
        var validatorMock = new Mock<IValidator<TestQuery>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestQuery>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestQuery, string>(validators);

        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync("ok");

        var result = await behavior.Handle(new TestQuery("x"), nextMock.Object, CancellationToken.None);

        Assert.Equal("ok", result);
        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidadoresFallaron_LanzaValidationException()
    {
        var validatorMock = new Mock<IValidator<TestQuery>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestQuery>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult([
                new FluentValidation.Results.ValidationFailure("Value", "Error test")
            ]));

        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestQuery, string>(validators);

        var nextMock = new Mock<RequestHandlerDelegate<string>>();

        await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(new TestQuery("x"), nextMock.Object, CancellationToken.None));

        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PasaCancellationTokenANext()
    {
        var validators = Enumerable.Empty<IValidator<TestQuery>>();
        var behavior = new ValidationBehavior<TestQuery, string>(validators);

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n(token)).ReturnsAsync("ok");

        await behavior.Handle(new TestQuery("x"), nextMock.Object, token);

        nextMock.Verify(n => n(token), Times.Once);
    }

    [Fact]
    public async Task Handle_PasaCancellationTokenAValidateAsync()
    {
        var validatorMock = new Mock<IValidator<TestQuery>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestQuery>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var behavior = new ValidationBehavior<TestQuery, string>([validatorMock.Object]);

        var cts = new CancellationTokenSource();
        var token = cts.Token;
        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync("ok");

        await behavior.Handle(new TestQuery("x"), nextMock.Object, token);

        validatorMock.Verify(v => v.ValidateAsync(
            It.IsAny<ValidationContext<TestQuery>>(), token), Times.Once);
    }
}
