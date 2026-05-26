using SiteManagement.Application.Abstractions.Payments;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;

namespace SiteManagement.Application.Billing.Services;

/// <summary>
/// Default <see cref="IBillItemPaymentService"/>: charges through the
/// <see cref="IPaymentGateway"/> port and turns a decline into a
/// <see cref="PaymentRejectedException"/> (→ 402). The card is forwarded to the
/// gateway and never persisted here.
/// </summary>
public sealed class BillItemPaymentService(IPaymentGateway paymentGateway) : IBillItemPaymentService
{
    private readonly IPaymentGateway _paymentGateway = paymentGateway;

    /// <inheritdoc />
    public async Task ChargeOrThrowAsync(string itemReference, decimal amount, CardDetails card, CancellationToken ct = default)
    {
        var result = await _paymentGateway.ChargeAsync(
            new ChargeRequest(
                IdempotencyKey: itemReference,
                CardNumber: card.CardNumber,
                Cvv: card.Cvv,
                ExpiryYear: card.ExpiryYear,
                ExpiryMonth: card.ExpiryMonth,
                Amount: amount,
                Reference: itemReference),
            ct);

        if (!result.Succeeded)
        {
            throw new PaymentRejectedException(
                ErrorMessageKeys.PaymentRejected,
                result.FailureReason ?? "rejected");
        }
    }
}
