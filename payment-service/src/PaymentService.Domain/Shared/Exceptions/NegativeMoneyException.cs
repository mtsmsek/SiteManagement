namespace PaymentService.Domain.Shared.Exceptions;

/// <summary>
/// Thrown when a <see cref="PaymentService.Domain.Shared.ValueObjects.Money"/>
/// would be created or computed with a negative amount. Balances and charges
/// are always non-negative.
/// </summary>
public sealed class NegativeMoneyException : DomainException
{
    /// <summary>Creates the exception for the offending amount.</summary>
    public NegativeMoneyException(decimal amount)
        : base(PaymentMessageKeys.MoneyNegative, amount)
    {
    }
}
