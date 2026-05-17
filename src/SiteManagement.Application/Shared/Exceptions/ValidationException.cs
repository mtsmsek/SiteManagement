using FluentValidation.Results;

namespace SiteManagement.Application.Shared.Exceptions;

/// <summary>
/// Thrown by <c>ValidationBehavior</c> when one or more FluentValidation
/// failures are collected before the handler runs. Renders as HTTP 400 with
/// an <c>errors</c> dictionary in the ProblemDetails extensions.
/// </summary>
public sealed class ValidationException : ApplicationException
{
    /// <summary>Default message returned with a 400 ProblemDetails body.</summary>
    public const string DefaultMessage = "One or more validation errors occurred.";

    /// <summary>Creates a validation exception from a flat list of FluentValidation failures.</summary>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base(DefaultMessage, HttpStatus.BadRequest)
    {
        Errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray());
    }

    /// <summary>Per-property error messages, keyed by request property path.</summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
