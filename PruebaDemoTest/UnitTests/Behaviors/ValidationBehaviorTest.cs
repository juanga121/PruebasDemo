using FluentValidation;
using MediatR;
using Moq;
using PruebasDemo.Application.Behaviors;

namespace PruebaDemoTest.UnitTests.Behaviors;

public record TestQuery(string Value) : IRequest<string>;

public class ValidationBehaviorTest
{
    [Fact]
    public async Task Handle_WithoutValidators_InvokesNext()
    {
        IEnumerable<IValidator<TestQuery>> validators = Enumerable.Empty<IValidator<TestQuery>>();
        var behavior = new ValidationBehavior<TestQuery, string>(validators);

        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(next => next(It.IsAny<CancellationToken>())).ReturnsAsync("ok");

        string result = await behavior.Handle(new TestQuery("x"), nextMock.Object, CancellationToken.None);

        Assert.Equal("ok", result);
        nextMock.Verify(next => next(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidatorsPassed_InvokesNext()
    {
        var validatorMock = new Mock<IValidator<TestQuery>>();
        validatorMock
            .Setup(validator => validator.ValidateAsync(It.IsAny<ValidationContext<TestQuery>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        IValidator<TestQuery>[] validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestQuery, string>(validators);

        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(next => next(It.IsAny<CancellationToken>())).ReturnsAsync("ok");

        string result = await behavior.Handle(new TestQuery("x"), nextMock.Object, CancellationToken.None);

        Assert.Equal("ok", result);
        nextMock.Verify(next => next(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidatorsFailed_ThrowsValidationException()
    {
        var validatorMock = new Mock<IValidator<TestQuery>>();
        validatorMock
            .Setup(validator => validator.ValidateAsync(It.IsAny<ValidationContext<TestQuery>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult([
                new FluentValidation.Results.ValidationFailure("Value", "Error test")
            ]));

        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestQuery, string>(validators);

        var nextMock = new Mock<RequestHandlerDelegate<string>>();

        await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(new TestQuery("x"), nextMock.Object, CancellationToken.None));

        nextMock.Verify(next => next(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PassesCancellationTokenToNext()
    {
        IEnumerable<IValidator<TestQuery>> validators = Enumerable.Empty<IValidator<TestQuery>>();
        var behavior = new ValidationBehavior<TestQuery, string>(validators);

        var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(next => next(token)).ReturnsAsync("ok");

        await behavior.Handle(new TestQuery("x"), nextMock.Object, token);

        nextMock.Verify(next => next(token), Times.Once);
    }

    [Fact]
    public async Task Handle_PassesCancellationTokenToValidateAsync()
    {
        var validatorMock = new Mock<IValidator<TestQuery>>();
        validatorMock
            .Setup(validator => validator.ValidateAsync(It.IsAny<ValidationContext<TestQuery>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var behavior = new ValidationBehavior<TestQuery, string>([validatorMock.Object]);

        var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(next => next(It.IsAny<CancellationToken>())).ReturnsAsync("ok");

        await behavior.Handle(new TestQuery("x"), nextMock.Object, token);

        validatorMock.Verify(validator => validator.ValidateAsync(
            It.IsAny<ValidationContext<TestQuery>>(), token), Times.Once);
    }
}
