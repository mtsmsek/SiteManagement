namespace SiteManagement.Infrastructure.Payments;

/// <summary>
/// Wire shape of the Payment service's <c>POST /api/payments</c> request. Lives
/// in Infrastructure (the adapter boundary) — the Application layer never sees
/// it; the adapter maps to/from the domain-friendly gateway contract.
/// </summary>
public sealed record PaymentServiceChargeRequest(
    string IdempotencyKey,
    string CardNumber,
    string Cvv,
    int ExpiryYear,
    int ExpiryMonth,
    decimal Amount,
    string Reference);

/// <summary>Wire shape of the Payment service's charge response.</summary>
public sealed record PaymentServiceChargeResponse(Guid TransactionId, string Status, string? FailureReason);
