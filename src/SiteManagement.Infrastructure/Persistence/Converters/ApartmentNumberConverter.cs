using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Converters;

/// <summary>Maps <see cref="ApartmentNumber"/> to/from its underlying int value.</summary>
public sealed class ApartmentNumberConverter : ValueConverter<ApartmentNumber, int>
{
    public ApartmentNumberConverter()
        : base(v => v.Value, v => ApartmentNumber.From(v))
    {
    }
}
