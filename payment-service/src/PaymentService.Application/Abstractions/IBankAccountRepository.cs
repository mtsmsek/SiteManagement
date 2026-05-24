using PaymentService.Domain;

namespace PaymentService.Application.Abstractions;

/// <summary>Persistence port for the <see cref="BankAccount"/> aggregate.</summary>
public interface IBankAccountRepository
{
    /// <summary>Loads an account by id, or null when it doesn't exist.</summary>
    Task<BankAccount?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Persists a new account.</summary>
    Task AddAsync(BankAccount account, CancellationToken ct = default);

    /// <summary>Persists the balance change of an existing account.</summary>
    Task UpdateAsync(BankAccount account, CancellationToken ct = default);
}
