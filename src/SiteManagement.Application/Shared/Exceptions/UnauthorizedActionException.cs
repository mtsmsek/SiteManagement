namespace SiteManagement.Application.Shared.Exceptions;

/// <summary>
/// Thrown when the caller is authenticated but not allowed to perform the
/// requested action — wrong role (raised centrally by <c>AuthorizationBehavior</c>)
/// or a resident reaching another resident's resource (raised by the handler
/// that owns the ownership check). Renders as HTTP 403. Carries a
/// <see cref="MessageKey"/> the Api middleware localizes (same contract as
/// <see cref="PaymentRejectedException"/>).
/// </summary>
public sealed class UnauthorizedActionException : ApplicationException
{
    /// <summary>Creates a 403 from a stable resource key the middleware localizes.</summary>
    /// <param name="messageKey">Stable resource key for the denial message.</param>
    public UnauthorizedActionException(string messageKey) : base(messageKey, HttpStatus.Forbidden)
    {
        MessageKey = messageKey;
    }

    /// <summary>Stable resource key for the denial message.</summary>
    public string MessageKey { get; }
}
