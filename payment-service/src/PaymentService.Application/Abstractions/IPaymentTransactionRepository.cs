using PaymentService.Domain;

namespace PaymentService.Application.Abstractions;

/// <summary>Persistence port for the <see cref="PaymentTransaction"/> aggregate.</summary>
public interface IPaymentTransactionRepository
{
    /// <summary>
    /// Finds the transaction recorded under an idempotency key, or null. The
    /// heart of idempotency: a repeated charge request resolves to the prior
    /// result instead of charging again.
    /// </summary>
    Task<PaymentTransaction?> FindByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct = default);

    /// <summary>Persists a new transaction.</summary>
    Task AddAsync(PaymentTransaction transaction, CancellationToken ct = default);
}
