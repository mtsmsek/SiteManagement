using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Application.Shared.Exceptions;

/// <summary>
/// Application-level translation of a <see cref="DomainException"/>. Renders
/// as HTTP 409 (Conflict) to convey "your request makes sense but violates a
/// business rule" — distinct from validation errors (400) and missing entities (404).
/// </summary>
public sealed class BusinessRuleViolationException : ApplicationException
{
    /// <summary>Creates a business-rule violation with a localized message.</summary>
    /// <param name="message">Localized message returned to the client.</param>
    /// <param name="messageKey">Stable resource key (useful for clients to react programmatically).</param>
    /// <param name="inner">Originating domain exception.</param>
    public BusinessRuleViolationException(string message, string messageKey, Exception? inner = null)
        : base(message, HttpStatus.Conflict, inner)
    {
        MessageKey = messageKey;
    }

    /// <summary>Stable resource key of the underlying domain violation.</summary>
    public string MessageKey { get; }
}
