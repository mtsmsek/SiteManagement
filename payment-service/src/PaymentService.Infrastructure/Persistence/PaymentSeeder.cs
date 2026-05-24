using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Abstractions;
using PaymentService.Domain;
using PaymentService.Domain.Shared.ValueObjects;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Infrastructure.Persistence;

/// <summary>
/// Seeds a demo card backed by a funded fake bank account so the payment flow
/// is exercisable out of the box. Idempotent: if the demo card already exists
/// (by number) it does nothing, so it is safe to run on every startup. This is
/// a fake gateway — in a real system cards/accounts are never seeded like this.
/// </summary>
public static class PaymentSeeder
{
    /// <summary>The demo card number (Luhn-valid Visa test PAN).</summary>
    public const string DemoCardNumber = "4242424242424242";

    private const decimal DemoOpeningBalance = 100_000m;
    private const string DemoCvv = "123";
    private const int DemoExpiryYear = 2030;
    private const int DemoExpiryMonth = 12;

    /// <summary>Creates the demo account + card if the card isn't already on file.</summary>
    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IBankAccountRepository>();
        var cards = scope.ServiceProvider.GetRequiredService<ICreditCardRepository>();

        var cardNumber = CardNumber.From(DemoCardNumber);
        if (await cards.FindByNumberAsync(cardNumber, ct) is not null)
        {
            return;
        }

        var account = BankAccount.Open(Money.Of(DemoOpeningBalance));
        await accounts.AddAsync(account, ct);

        var card = CreditCard.Issue(
            account.Id,
            cardNumber,
            Cvv.From(DemoCvv),
            ExpiryDate.Of(DemoExpiryYear, DemoExpiryMonth));
        await cards.AddAsync(card, ct);
    }
}
