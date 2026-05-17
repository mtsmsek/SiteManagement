using FluentValidation.Results;

namespace SiteManagement.Application.Shared.Exceptions;

/// <summary>
/// Thrown by <c>ValidationBehavior</c> when one or more FluentValidation
/// failures are collected before the handler runs. Renders as HTTP 400 with
/// an <c>errors</c> dictionary in the ProblemDetails extensions.
/// </summary>
/// <remarks>
/// Each failure travels alongside its FluentValidation placeholder values
/// (e.g. <c>PropertyName</c>, <c>MaxLength</c>) so the API middleware can
/// re-format the localized template after looking the key up.
/// </remarks>
public sealed class ValidationException : ApplicationException
{
    /// <summary>Default message returned with a 400 ProblemDetails body.</summary>
    public const string DefaultMessage = "One or more validation errors occurred.";

    /// <summary>Creates a validation exception from a flat list of FluentValidation failures.</summary>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base(DefaultMessage, HttpStatus.BadRequest)
    {
        Failures = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => new ValidationFailureDetail(
                        f.ErrorMessage,
                        f.FormattedMessagePlaceholderValues ?? new Dictionary<string, object>()))
                    .ToArray());
    }

    /// <summary>
    /// Per-property failures. Each value holds the FluentValidation message
    /// (which is either a localization key or a literal) and the placeholder
    /// dictionary needed to expand <c>{PropertyName}</c> etc.
    /// </summary>
    public IReadOnlyDictionary<string, ValidationFailureDetail[]> Failures { get; }
}

/// <summary>One FluentValidation failure, decoupled from the FV types.</summary>
public sealed record ValidationFailureDetail(
    string MessageKey,
    IReadOnlyDictionary<string, object> Placeholders);
