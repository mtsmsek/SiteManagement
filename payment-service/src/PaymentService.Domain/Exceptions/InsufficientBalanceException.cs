using PaymentService.Domain.Shared.Exceptions;

namespace PaymentService.Domain.Exceptions;

/// <summary>
/// Thrown when a debit would take a <c>BankAccount</c> below zero — the fake
/// bank's "insufficient funds" decline.
/// </summary>
public sealed class InsufficientBalanceException : DomainException
{
    /// <summary>Creates the exception for the account that lacked funds.</summary>
    public InsufficientBalanceException(Guid accountId)
        : base(PaymentMessageKeys.InsufficientBalance, accountId)
    {
    }
}
