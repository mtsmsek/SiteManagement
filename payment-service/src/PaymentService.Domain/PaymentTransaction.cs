using PaymentService.Domain.Exceptions;
using PaymentService.Domain.Shared;
using PaymentService.Domain.Shared.ValueObjects;

namespace PaymentService.Domain;

/// <summary>
/// Aggregate root recording one charge attempt. The <see cref="IdempotencyKey"/>
/// makes a retry safe: the Application looks a transaction up by key and returns
/// the existing result instead of charging again. <see cref="Reference"/> is an
/// opaque string from the caller (the main API's billing-item id) — this service
/// never interprets it, keeping it a generic payment gateway. Settles exactly
/// once to Succeeded or Failed.
/// </summary>
public sealed class PaymentTransaction : AggregateRoot<Guid>
{
    /// <summary>Caller-supplied key that makes repeated charge requests idempotent.</summary>
    public string IdempotencyKey { get; private set; }

    /// <summary>Opaque caller reference (e.g. the main API's billing-item id).</summary>
    public string Reference { get; private set; }

    /// <summary>The amount charged.</summary>
    public Money Amount { get; private set; }

    /// <summary>Current lifecycle state.</summary>
    public PaymentStatus Status { get; private set; }

    /// <summary>Decline reason when <see cref="Status"/> is Failed; otherwise null.</summary>
    public string? FailureReason { get; private set; }

    // Persistence materialisation ctor.
    private PaymentTransaction()
    {
        IdempotencyKey = string.Empty;
        Reference = string.Empty;
        Amount = default!;
    }

    private PaymentTransaction(Guid id, string idempotencyKey, string reference, Money amount) : base(id)
    {
        IdempotencyKey = idempotencyKey;
        Reference = reference;
        Amount = amount;
        Status = PaymentStatus.Pending;
    }

    /// <summary>Starts a pending transaction for a charge attempt.</summary>
    public static PaymentTransaction Start(string idempotencyKey, string reference, Money amount)
        => new(Guid.NewGuid(), idempotencyKey, reference, amount);

    /// <summary>Settles the transaction as succeeded.</summary>
    /// <exception cref="InvalidTransactionStateException">Thrown when already settled.</exception>
    public void Succeed()
    {
        EnsurePending();
        Status = PaymentStatus.Succeeded;
    }

    /// <summary>Settles the transaction as failed with a reason.</summary>
    /// <exception cref="InvalidTransactionStateException">Thrown when already settled.</exception>
    public void Fail(string reason)
    {
        EnsurePending();
        Status = PaymentStatus.Failed;
        FailureReason = reason;
    }

    private void EnsurePending()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidTransactionStateException(Id);
        }
    }
}
