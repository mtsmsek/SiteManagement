namespace PaymentService.Domain;

/// <summary>
/// Lifecycle of a <see cref="PaymentTransaction"/>: created <see cref="Pending"/>,
/// then settled to <see cref="Succeeded"/> or <see cref="Failed"/> exactly once.
/// </summary>
public enum PaymentStatus
{
    /// <summary>Created but not yet settled.</summary>
    Pending = 0,

    /// <summary>Charge succeeded (account debited).</summary>
    Succeeded = 1,

    /// <summary>Charge declined (insufficient funds, rejected card, …).</summary>
    Failed = 2,
}
