namespace SiteManagement.Application.Abstractions.Payments;

/// <summary>
/// Application port for charging a card through the external Payment service.
/// The domain/handlers depend on this — not on HTTP, Refit, or the payment
/// service's URL — so the transport (and one day the provider) can change
/// behind an adapter without touching the use case. An anti-corruption layer:
/// the gateway's wire shape is translated into this domain-friendly contract.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// Charges <paramref name="amount"/> to the card. <paramref name="idempotencyKey"/>
    /// makes a retry safe (the gateway returns the original result instead of
    /// charging again); <paramref name="reference"/> is an opaque correlation
    /// string (the billing item id).
    /// </summary>
    Task<PaymentResult> ChargeAsync(ChargeRequest request, CancellationToken ct = default);
}

/// <summary>A charge to attempt: card details, amount, and the idempotency/reference keys.</summary>
public sealed record ChargeRequest(
    string IdempotencyKey,
    string CardNumber,
    string Cvv,
    int ExpiryYear,
    int ExpiryMonth,
    decimal Amount,
    string Reference);

/// <summary>The gateway's domain-friendly outcome.</summary>
/// <param name="Succeeded">True when the charge settled successfully.</param>
/// <param name="FailureReason">Decline reason when not succeeded; otherwise null.</param>
public sealed record PaymentResult(bool Succeeded, string? FailureReason);
