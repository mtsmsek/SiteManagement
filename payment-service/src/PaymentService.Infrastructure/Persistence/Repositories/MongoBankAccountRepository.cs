using MongoDB.Driver;
using PaymentService.Application.Abstractions;
using PaymentService.Domain;
using PaymentService.Domain.Shared.ValueObjects;
using PaymentService.Infrastructure.Persistence.Documents;

namespace PaymentService.Infrastructure.Persistence.Repositories;

/// <summary>
/// MongoDB-backed <see cref="IBankAccountRepository"/>. Maps between the domain
/// aggregate and its <see cref="BankAccountDocument"/> persistence shape so the
/// Domain layer stays free of any Mongo concern.
/// </summary>
public sealed class MongoBankAccountRepository(PaymentMongoContext context) : IBankAccountRepository
{
    private const string CollectionName = "bank_accounts";

    private readonly IMongoCollection<BankAccountDocument> _collection =
        context.Collection<BankAccountDocument>(CollectionName);

    /// <inheritdoc />
    public async Task<BankAccount?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var doc = await _collection.Find(d => d.Id == id).FirstOrDefaultAsync(ct);
        return doc is null ? null : ToDomain(doc);
    }

    /// <inheritdoc />
    public Task AddAsync(BankAccount account, CancellationToken ct = default)
        => _collection.InsertOneAsync(ToDocument(account), options: null, ct);

    /// <inheritdoc />
    public Task UpdateAsync(BankAccount account, CancellationToken ct = default)
        => _collection.ReplaceOneAsync(d => d.Id == account.Id, ToDocument(account), options: (ReplaceOptions?)null, ct);

    private static BankAccountDocument ToDocument(BankAccount account) => new()
    {
        Id = account.Id,
        BalanceAmount = account.Balance.Amount,
        BalanceCurrency = account.Balance.Currency,
    };

    private static BankAccount ToDomain(BankAccountDocument doc)
        => BankAccount.Restore(doc.Id, Money.Of(doc.BalanceAmount));
}
