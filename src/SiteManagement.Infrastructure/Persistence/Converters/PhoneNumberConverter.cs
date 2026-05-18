using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Converters;

/// <summary>Maps <see cref="PhoneNumber"/> to/from its canonical <c>+90...</c> string.</summary>
public sealed class PhoneNumberConverter : ValueConverter<PhoneNumber, string>
{
    public PhoneNumberConverter()
        : base(v => v.Value, v => PhoneNumber.From(v))
    {
    }
}
