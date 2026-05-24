namespace PaymentService.Infrastructure.Persistence.Documents;

/// <summary>
/// MongoDB persistence shape for a BankAccount. Kept separate from the domain
/// aggregate so the Domain layer carries no Mongo/serialization concern; the
/// repository maps between the two. Money is flattened to amount + currency.
/// </summary>
public sealed class BankAccountDocument
{
    public Guid Id { get; set; }
    public decimal BalanceAmount { get; set; }
    public string BalanceCurrency { get; set; } = string.Empty;
}
