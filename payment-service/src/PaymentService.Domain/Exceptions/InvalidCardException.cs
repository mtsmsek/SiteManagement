using PaymentService.Domain.Shared.Exceptions;

namespace PaymentService.Domain.Exceptions;

/// <summary>
/// Thrown when card details fail surface validation: bad PAN (length/Luhn),
/// bad CVV, or an out-of-range expiry month. Distinct from
/// <see cref="CardRejectedException"/>, which is a business decline at charge time.
/// </summary>
public sealed class InvalidCardException : DomainException
{
    /// <summary>Creates the exception describing which part was invalid.</summary>
    public InvalidCardException(string detail)
        : base(PaymentMessageKeys.CardInvalid, detail)
    {
    }
}
