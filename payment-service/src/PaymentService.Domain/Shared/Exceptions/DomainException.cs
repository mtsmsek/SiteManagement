namespace PaymentService.Domain.Shared.Exceptions;

/// <summary>
/// Base class for all PaymentService domain-layer invariant violations. Carries
/// a stable <see cref="MessageKey"/> the Application layer can map to an HTTP
/// problem. A deliberate copy of the main API's pattern — the services share no
/// code, only HTTP contracts.
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>Creates a domain exception with a resource key and optional format arguments.</summary>
    /// <param name="messageKey">Stable key identifying the violation.</param>
    /// <param name="messageArgs">Format arguments for the localized template.</param>
    protected DomainException(string messageKey, params object[] messageArgs)
        : base(messageKey)
    {
        MessageKey = messageKey;
        MessageArgs = messageArgs;
    }

    /// <summary>Stable resource key (e.g. <c>"Payment.InsufficientBalance"</c>).</summary>
    public string MessageKey { get; }

    /// <summary>Format arguments substituted into the localized message template.</summary>
    public IReadOnlyList<object> MessageArgs { get; }
}
