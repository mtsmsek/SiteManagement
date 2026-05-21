namespace SiteManagement.Domain.Shared.Exceptions;

/// <summary>
/// Thrown when a <see cref="SiteManagement.Domain.Shared.ValueObjects.Money"/>
/// would be created or computed with a negative amount. Money in this domain
/// is always non-negative; debts are modelled as separate billing items, not
/// as negative balances.
/// </summary>
public sealed class NegativeMoneyException : DomainException
{
    /// <summary>Creates the exception for the offending amount.</summary>
    /// <param name="amount">The negative amount that was rejected.</param>
    public NegativeMoneyException(decimal amount)
        : base(SharedMessageKeys.MoneyNegative, amount)
    {
    }
}
