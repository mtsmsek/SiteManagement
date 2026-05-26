namespace SiteManagement.Application.Billing.Services;

/// <summary>
/// Charges a single billing item through the payment gateway, shared by every
/// pay path (admin dues/utility + resident self-service) so the
/// charge-then-throw-on-decline logic and the idempotency key format live in
/// one place rather than being copied into each handler.
/// </summary>
public interface IBillItemPaymentService
{
    /// <summary>
    /// Charges <paramref name="amount"/> for the item identified by
    /// <paramref name="itemReference"/> (used as both the gateway idempotency
    /// key and the opaque reference). Throws
    /// <see cref="Shared.Exceptions.PaymentRejectedException"/> on a decline;
    /// returns normally on success. Never marks anything paid — the caller owns
    /// the aggregate state change.
    /// </summary>
    Task ChargeOrThrowAsync(string itemReference, decimal amount, CardDetails card, CancellationToken ct = default);
}

/// <summary>The card a resident/admin supplies for a charge; passed straight to the gateway, never stored.</summary>
public sealed record CardDetails(string CardNumber, string Cvv, int ExpiryYear, int ExpiryMonth);

/// <summary>
/// Builds the deterministic per-item key used as both the gateway idempotency
/// key and reference, so a retry of the same item resolves to the same charge.
/// Centralized here to avoid the <c>"dues-item:"</c> / <c>"utility-item:"</c>
/// literals drifting between handlers.
/// </summary>
public static class BillItemReference
{
    /// <summary>Reference for a dues item.</summary>
    public static string ForDuesItem(Guid itemId) => $"dues-item:{itemId}";

    /// <summary>Reference for a utility bill item.</summary>
    public static string ForUtilityItem(Guid itemId) => $"utility-item:{itemId}";
}
