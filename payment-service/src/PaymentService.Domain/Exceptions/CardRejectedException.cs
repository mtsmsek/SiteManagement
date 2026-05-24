using PaymentService.Domain.Shared.Exceptions;

namespace PaymentService.Domain.Exceptions;

/// <summary>
/// Thrown when a card is declined at charge time for a business reason
/// (expired). Distinct from <see cref="InvalidCardException"/>, which is a
/// surface-format failure before the charge is attempted.
/// </summary>
public sealed class CardRejectedException : DomainException
{
    /// <summary>Creates the exception describing the decline reason.</summary>
    public CardRejectedException(string reason)
        : base(PaymentMessageKeys.CardRejected, reason)
    {
    }
}
