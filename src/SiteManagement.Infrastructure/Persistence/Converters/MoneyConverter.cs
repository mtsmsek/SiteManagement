using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Converters;

/// <summary>
/// Maps <see cref="Money"/> to/from its decimal amount. Currency is fixed
/// (TRY) so only the amount is persisted; <see cref="Money.Of"/> re-applies
/// the non-negative + rounding invariants on read.
/// </summary>
public sealed class MoneyConverter : ValueConverter<Money, decimal>
{
    public MoneyConverter()
        : base(m => m.Amount, v => Money.Of(v))
    {
    }
}
