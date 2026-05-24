using PaymentService.Domain;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Abstractions;

/// <summary>Persistence port for the <see cref="CreditCard"/> aggregate.</summary>
public interface ICreditCardRepository
{
    /// <summary>Finds the card matching the supplied number, or null when none is on file.</summary>
    Task<CreditCard?> FindByNumberAsync(CardNumber number, CancellationToken ct = default);

    /// <summary>Persists a new card.</summary>
    Task AddAsync(CreditCard card, CancellationToken ct = default);
}
