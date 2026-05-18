using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Converters;

/// <summary>
/// Maps <see cref="ApartmentType"/> to/from its canonical <c>"N+M"</c> string
/// for storage so the column reads like the domain ubiquitous language
/// instead of two anonymous int columns.
/// </summary>
public sealed class ApartmentTypeConverter : ValueConverter<ApartmentType, string>
{
    public ApartmentTypeConverter()
        : base(v => v.ToString(), v => ApartmentType.From(v))
    {
    }
}
