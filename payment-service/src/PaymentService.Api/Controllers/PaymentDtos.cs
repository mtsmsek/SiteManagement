using PaymentService.Application.Payments;

namespace PaymentService.Api.Controllers;

/// <summary>
/// Charge request body for <c>POST /api/payments</c>. Card fields are raw;
/// the processor validates them into value objects. Kept distinct from the
/// application <see cref="ProcessPaymentRequest"/> so the HTTP contract can
/// evolve independently of the use-case input; the conversion lives on this
/// DTO (not in the controller) so the controller stays a thin pass-through.
/// </summary>
public sealed record ProcessPaymentApiRequest(
    string IdempotencyKey,
    string CardNumber,
    string Cvv,
    int ExpiryYear,
    int ExpiryMonth,
    decimal Amount,
    string Reference)
{
    /// <summary>Maps this HTTP request to the application use-case input.</summary>
    public ProcessPaymentRequest ToApplicationRequest()
        => new(IdempotencyKey, CardNumber, Cvv, ExpiryYear, ExpiryMonth, Amount, Reference);
}

/// <summary>Charge result returned by <c>POST /api/payments</c>.</summary>
public sealed record ProcessPaymentApiResponse(Guid TransactionId, string Status, string? FailureReason)
{
    /// <summary>Maps an application result to the HTTP response shape.</summary>
    public static ProcessPaymentApiResponse From(ProcessPaymentResult result)
        => new(result.TransactionId, result.Status, result.FailureReason);
}
