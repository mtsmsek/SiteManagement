using FluentValidation;
using MediatR;
using ValidationException = SiteManagement.Application.Shared.Exceptions.ValidationException;

namespace SiteManagement.Application.Behaviors;

/// <summary>
/// Runs every registered <see cref="IValidator{T}"/> for the incoming request
/// before the handler executes. If any validator returns failures, throws a
/// <see cref="ValidationException"/> aggregating them — the handler never
/// runs with invalid input.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToArray();

        if (failures.Length > 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
