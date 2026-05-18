using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Infrastructure.Persistence.Converters;

/// <summary>Maps <see cref="TcNo"/> to/from its 11-digit string representation.</summary>
public sealed class TcNoConverter : ValueConverter<TcNo, string>
{
    public TcNoConverter()
        : base(v => v.Value, v => TcNo.From(v))
    {
    }
}
