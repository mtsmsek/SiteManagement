using Refit;

namespace SiteManagement.Infrastructure.Payments;

/// <summary>
/// Refit-generated HTTP client for the external Payment service — an OUTBOUND
/// call to another system, so it lives in Infrastructure alongside the DB and
/// mail adapters. Refit turns this interface into the actual HTTP calls at
/// runtime; the API-key header is attached by the HttpClient configured in DI.
/// Internal to Infrastructure: the <see cref="PaymentGatewayAdapter"/> wraps it
/// behind the Application's <c>IPaymentGateway</c> port so handlers stay
/// transport-agnostic.
/// </summary>
public interface IPaymentServiceApi
{
    /// <summary>Posts a charge to the Payment service.</summary>
    [Post("/api/payments")]
    Task<PaymentServiceChargeResponse> ChargeAsync(
        [Body] PaymentServiceChargeRequest request,
        CancellationToken ct = default);
}
