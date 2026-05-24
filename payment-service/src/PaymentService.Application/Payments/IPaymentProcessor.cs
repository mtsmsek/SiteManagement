namespace PaymentService.Application.Payments;

/// <summary>
/// The service's single use case: process a card charge. Idempotent by the
/// request's key — a repeated call returns the original result without
/// charging again.
/// </summary>
public interface IPaymentProcessor
{
    /// <summary>Charges the card (or returns a prior result for a repeated key).</summary>
    Task<ProcessPaymentResult> ProcessAsync(ProcessPaymentRequest request, CancellationToken ct = default);
}
