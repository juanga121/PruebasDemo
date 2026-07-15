using FluentValidation;
using FluentValidation.Results;

namespace PruebasDemo.Application.Behaviors;

internal static class ValidationBehaviorHelper
{
    public static async Task ValidateAsync<TRequest>(
        IEnumerable<IValidator<TRequest>> validators,
        TRequest request,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return;

        var context = new ValidationContext<TRequest>(request);
        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        List<ValidationFailure> failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);
    }
}
