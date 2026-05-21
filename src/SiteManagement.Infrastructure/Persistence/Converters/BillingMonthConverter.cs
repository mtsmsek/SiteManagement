using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteManagement.Domain.Billing.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Converters;

/// <summary>
/// Maps <see cref="BillingMonth"/> to/from its "yyyy-MM" string form. Stored
/// as text so the column is human-readable and naturally chronologically
/// sortable; the value object re-validates the parts on read.
/// </summary>
public sealed class BillingMonthConverter : ValueConverter<BillingMonth, string>
{
    public BillingMonthConverter()
        : base(
            m => m.ToString(),
            s => BillingMonth.Of(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(5, 2))))
    {
    }
}
