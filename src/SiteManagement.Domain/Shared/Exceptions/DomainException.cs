namespace SiteManagement.Domain.Shared.Exceptions;

/// <summary>
/// Base class for all domain-layer invariant violations. Carries a stable
/// <see cref="MessageKey"/> that the Application layer translates into a
/// localized message before re-throwing as an <c>ApplicationException</c>.
/// Domain exceptions MUST NOT leak past the Application boundary.
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Creates a domain exception with a resource key and optional format arguments.
    /// </summary>
    /// <param name="messageKey">Resource key used by <c>IStringLocalizer</c>.</param>
    /// <param name="messageArgs">Format arguments substituted into the localized template.</param>
    protected DomainException(string messageKey, params object[] messageArgs)
        : base(messageKey)
    {
        MessageKey = messageKey;
        MessageArgs = messageArgs;
    }

    /// <summary>Stable resource key (e.g. <c>"Apartment.AlreadyOccupied"</c>).</summary>
    public string MessageKey { get; }

    /// <summary>Format arguments substituted into the localized message template.</summary>
    public IReadOnlyList<object> MessageArgs { get; }
}
