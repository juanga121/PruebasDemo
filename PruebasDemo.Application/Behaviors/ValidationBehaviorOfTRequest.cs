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
        await ValidationBehaviorHelper.ValidateAsync(_validators, request, cancellationToken);
        return await next(cancellationToken);
    }
}
