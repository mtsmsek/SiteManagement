namespace SiteManagement.Application.Shared.Exceptions;

/// <summary>
/// Thrown when the payment gateway declines a charge (insufficient funds,
/// rejected card, …). Renders as HTTP 402 (Payment Required): the request was
/// valid but the payment did not go through, so the billing item is left
/// unpaid. Distinct from 409 — this isn't a domain-rule violation, it's a
/// failed external charge. Carries a <see cref="MessageKey"/> the Api
/// middleware localizes (same contract as BusinessRuleViolationException).
/// </summary>
public sealed class PaymentRejectedException : ApplicationException
{
    /// <summary>Creates the exception with a message key + the gateway's decline reason.</summary>
    /// <param name="messageKey">Stable resource key the middleware localizes.</param>
    /// <param name="reason">Machine-readable decline reason (e.g. "insufficient_balance").</param>
    public PaymentRejectedException(string messageKey, string reason)
        : base(messageKey, HttpStatus.PaymentRequired)
    {
        MessageKey = messageKey;
        Reason = reason;
    }

    /// <summary>Stable resource key for the decline message.</summary>
    public string MessageKey { get; }

    /// <summary>The gateway's machine-readable decline reason.</summary>
    public string Reason { get; }
}
