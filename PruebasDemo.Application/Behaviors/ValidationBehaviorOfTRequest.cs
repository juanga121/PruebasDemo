using FluentValidation;
using MediatR;

namespace PruebasDemo.Application.Behaviors;

public class ValidationBehavior<TRequest>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, Unit>
    where TRequest : IRequest
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<Unit> Handle(
        TRequest request,
        RequestHandlerDelegate<Unit> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
                return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next(cancellationToken);
    }
}
