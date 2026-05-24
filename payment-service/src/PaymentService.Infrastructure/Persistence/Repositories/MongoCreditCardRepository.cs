using MongoDB.Driver;
using PaymentService.Application.Abstractions;
using PaymentService.Domain;
using PaymentService.Domain.ValueObjects;
using PaymentService.Infrastructure.Persistence.Documents;

namespace PaymentService.Infrastructure.Persistence.Repositories;

/// <summary>
/// MongoDB-backed <see cref="ICreditCardRepository"/>. Maps between the domain
/// aggregate and its <see cref="CreditCardDocument"/> persistence shape so the
/// Domain layer stays free of any Mongo concern.
/// </summary>
public sealed class MongoCreditCardRepository(PaymentMongoContext context) : ICreditCardRepository
{
    private const string CollectionName = "credit_cards";

    private readonly IMongoCollection<CreditCardDocument> _collection =
        context.Collection<CreditCardDocument>(CollectionName);

    /// <inheritdoc />
    public async Task<CreditCard?> FindByNumberAsync(CardNumber number, CancellationToken ct = default)
    {
        var doc = await _collection.Find(d => d.Number == number.Value).FirstOrDefaultAsync(ct);
        return doc is null ? null : ToDomain(doc);
    }

    /// <inheritdoc />
    public Task AddAsync(CreditCard card, CancellationToken ct = default)
        => _collection.InsertOneAsync(ToDocument(card), options: null, ct);

    private static CreditCardDocument ToDocument(CreditCard card) => new()
    {
        Id = card.Id,
        BankAccountId = card.BankAccountId,
        Number = card.Number.Value,
        Cvv = card.Cvv.Value,
        ExpiryYear = card.Expiry.Year,
        ExpiryMonth = card.Expiry.Month,
    };

    private static CreditCard ToDomain(CreditCardDocument doc)
        => CreditCard.Restore(
            doc.Id,
            doc.BankAccountId,
            CardNumber.From(doc.Number),
            Cvv.From(doc.Cvv),
            ExpiryDate.Of(doc.ExpiryYear, doc.ExpiryMonth));
}
