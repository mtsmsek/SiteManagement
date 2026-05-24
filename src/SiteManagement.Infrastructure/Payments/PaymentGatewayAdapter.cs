using SiteManagement.Application.Abstractions.Payments;

namespace SiteManagement.Infrastructure.Payments;

/// <summary>
/// Adapts the Application's <see cref="IPaymentGateway"/> port to the external
/// Payment service's HTTP client. Translates the domain-friendly
/// <see cref="ChargeRequest"/>/<see cref="PaymentResult"/> to and from the
/// service's wire contract (the anti-corruption layer), so the gateway's shape
/// never leaks into the Application layer. Resilience (retry/timeout/circuit
/// breaker) is configured on the HttpClient in DI, not here.
/// </summary>
public sealed class PaymentGatewayAdapter(IPaymentServiceApi api) : IPaymentGateway
{
    private const string SucceededStatus = "Succeeded";

    private readonly IPaymentServiceApi _api = api;

    /// <inheritdoc />
    public async Task<PaymentResult> ChargeAsync(ChargeRequest request, CancellationToken ct = default)
    {
        var response = await _api.ChargeAsync(
            new PaymentServiceChargeRequest(
                request.IdempotencyKey,
                request.CardNumber,
                request.Cvv,
                request.ExpiryYear,
                request.ExpiryMonth,
                request.Amount,
                request.Reference),
            ct);

        var succeeded = string.Equals(response.Status, SucceededStatus, StringComparison.Ordinal);
        return new PaymentResult(succeeded, succeeded ? null : response.FailureReason);
    }
}
