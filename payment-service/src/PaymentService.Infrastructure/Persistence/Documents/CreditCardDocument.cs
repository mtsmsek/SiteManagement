namespace PaymentService.Infrastructure.Persistence.Documents;

/// <summary>
/// MongoDB persistence shape for a CreditCard. Kept separate from the domain
/// aggregate so the Domain layer carries no Mongo/serialization concern; the
/// repository maps between the two, flattening the card value objects.
/// </summary>
public sealed class CreditCardDocument
{
    public Guid Id { get; set; }
    public Guid BankAccountId { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
    public int ExpiryYear { get; set; }
    public int ExpiryMonth { get; set; }
}
