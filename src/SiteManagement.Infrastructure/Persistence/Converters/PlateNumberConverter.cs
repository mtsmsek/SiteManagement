using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Converters;

/// <summary>Maps <see cref="PlateNumber"/> to/from its uppercase canonical string.</summary>
public sealed class PlateNumberConverter : ValueConverter<PlateNumber, string>
{
    public PlateNumberConverter()
        : base(v => v.Value, v => PlateNumber.From(v))
    {
    }
}
