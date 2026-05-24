using MongoDB.Driver;
using PaymentService.Application.Abstractions;
using PaymentService.Domain;
using PaymentService.Domain.Shared.ValueObjects;
using PaymentService.Infrastructure.Persistence.Documents;

namespace PaymentService.Infrastructure.Persistence.Repositories;

/// <summary>
/// MongoDB-backed <see cref="IPaymentTransactionRepository"/>. Maps between the
/// domain aggregate and its <see cref="PaymentTransactionDocument"/> persistence
/// shape so the Domain layer stays free of any Mongo concern.
/// </summary>
public sealed class MongoPaymentTransactionRepository : IPaymentTransactionRepository
{
    private const string CollectionName = "payment_transactions";

    private readonly IMongoCollection<PaymentTransactionDocument> _collection;

    /// <summary>
    /// Resolves the collection and guarantees a unique index on the idempotency
    /// key. This is the last line of defence against duplicate charges: even if
    /// two concurrent requests race past the application-level lookup, the unique
    /// index lets at most one insert win and the other fails loudly at the DB.
    /// <see cref="IMongoIndexManager{TDocument}.CreateOne(CreateIndexModel{TDocument}, CreateOneIndexOptions, CancellationToken)"/>
    /// is idempotent in MongoDB, so calling it on every construction is safe.
    /// </summary>
    public MongoPaymentTransactionRepository(PaymentMongoContext context)
    {
        _collection = context.Collection<PaymentTransactionDocument>(CollectionName);

        _collection.Indexes.CreateOne(new CreateIndexModel<PaymentTransactionDocument>(
            Builders<PaymentTransactionDocument>.IndexKeys.Ascending(d => d.IdempotencyKey),
            new CreateIndexOptions { Unique = true }));
    }

    /// <inheritdoc />
    public async Task<PaymentTransaction?> FindByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct = default)
    {
        var doc = await _collection.Find(d => d.IdempotencyKey == idempotencyKey).FirstOrDefaultAsync(ct);
        return doc is null ? null : ToDomain(doc);
    }

    /// <inheritdoc />
    public Task AddAsync(PaymentTransaction transaction, CancellationToken ct = default)
        => _collection.InsertOneAsync(ToDocument(transaction), options: null, ct);

    private static PaymentTransactionDocument ToDocument(PaymentTransaction transaction) => new()
    {
        Id = transaction.Id,
        IdempotencyKey = transaction.IdempotencyKey,
        Reference = transaction.Reference,
        Amount = transaction.Amount.Amount,
        Currency = transaction.Amount.Currency,
        Status = transaction.Status.ToString(),
        FailureReason = transaction.FailureReason,
    };

    private static PaymentTransaction ToDomain(PaymentTransactionDocument doc)
        => PaymentTransaction.Restore(
            doc.Id,
            doc.IdempotencyKey,
            doc.Reference,
            Money.Of(doc.Amount),
            Enum.Parse<PaymentStatus>(doc.Status),
            doc.FailureReason);
}
