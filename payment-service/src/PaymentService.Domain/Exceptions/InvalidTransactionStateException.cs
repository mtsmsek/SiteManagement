using PaymentService.Domain.Shared.Exceptions;

namespace PaymentService.Domain.Exceptions;

/// <summary>
/// Thrown when a <see cref="PaymentTransaction"/> is settled twice — a charge
/// result is recorded exactly once.
/// </summary>
public sealed class InvalidTransactionStateException : DomainException
{
    /// <summary>Creates the exception for the offending transition.</summary>
    public InvalidTransactionStateException(Guid transactionId)
        : base(PaymentMessageKeys.TransactionInvalidState, transactionId)
    {
    }
}
