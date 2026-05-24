namespace PaymentService.Application.Payments;

/// <summary>
/// A charge request: which card, how much, an opaque caller reference, and the
/// idempotency key that makes a retry safe. Card fields are raw strings here;
/// the processor turns them into validated value objects.
/// </summary>
public sealed record ProcessPaymentRequest(
    string IdempotencyKey,
    string CardNumber,
    string Cvv,
    int ExpiryYear,
    int ExpiryMonth,
    decimal Amount,
    string Reference);

/// <summary>The outcome of a charge: the transaction id and its settled status.</summary>
/// <param name="TransactionId">The recorded transaction's id.</param>
/// <param name="Status">"Succeeded" or "Failed".</param>
/// <param name="FailureReason">Set when the charge failed; otherwise null.</param>
public sealed record ProcessPaymentResult(Guid TransactionId, string Status, string? FailureReason);
