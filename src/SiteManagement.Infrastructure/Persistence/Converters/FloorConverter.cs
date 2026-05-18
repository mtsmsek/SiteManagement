using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Converters;

/// <summary>Maps <see cref="Floor"/> to/from its signed int value.</summary>
public sealed class FloorConverter : ValueConverter<Floor, int>
{
    public FloorConverter()
        : base(v => v.Value, v => Floor.From(v))
    {
    }
}
